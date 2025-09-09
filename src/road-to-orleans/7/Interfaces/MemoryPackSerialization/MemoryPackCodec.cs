using MemoryPack;
using Microsoft.Extensions.Options;
using Orleans.Serialization.Buffers;
using Orleans.Serialization.Buffers.Adaptors;
using Orleans.Serialization.Cloning;
using Orleans.Serialization.Codecs;
using Orleans.Serialization.Serializers;
using Orleans.Serialization.WireProtocol;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Orleans.Serialization;

/// <summary>
/// A serialization codec which uses <see cref="MemoryPackSerializer"/>.
/// </summary>
[Alias(WellKnownAlias)]
public sealed class MemoryPackCodec : IGeneralizedCodec, IGeneralizedCopier, ITypeFilter
{

    #region Constants & Statics

    /// <summary>
    /// The well-known type alias for this codec.
    /// </summary>
    [SuppressMessage(
        "Critical Code Smell",
        "S2339:Public constant members should not be used",
        Justification = "used for Attribute parameter")]
    public const string WellKnownAlias = "mempack";

    private static readonly Type SelfType = typeof(MemoryPackCodec);
    private static readonly ConcurrentDictionary<Type, bool> SupportedTypes = new();

    private static bool IsMemoryPackContract(Type type)
    {
        if (SupportedTypes.TryGetValue(type, out var isMemPackContract))
        {
            return isMemPackContract;
        }

        isMemPackContract = type.GetCustomAttribute<MemoryPackableAttribute>() is not null;

        _ = SupportedTypes.TryAdd(type, isMemPackContract);
        return isMemPackContract;
    }

    [DoesNotReturn]
    private static void ThrowTypeFieldMissing()
    {
        throw new RequiredFieldMissingException("Serialized value is missing its type field.");
    }

    #endregion

    private readonly ICopierSelector[] _copyableTypeSelectors;
    private readonly ICodecSelector[] _serializableTypeSelectors;
    private readonly MemoryPackCodecOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryPackCodec"/> class.
    /// </summary>
    /// /// <param name="serializableTypeSelectors">Filters used to indicate which types should be serialized by this codec.</param>
    /// <param name="copyableTypeSelectors">Filters used to indicate which types should be copied by this codec.</param>
    /// <param name="options">The MemoryPack codec options.</param>
    public MemoryPackCodec(
        IEnumerable<ICodecSelector> serializableTypeSelectors,
        IEnumerable<ICopierSelector> copyableTypeSelectors,
        IOptions<MemoryPackCodecOptions> options)
    {
        _serializableTypeSelectors = serializableTypeSelectors.Where(
            t => string.Equals(t.CodecName, WellKnownAlias, StringComparison.Ordinal))
            .ToArray();
        _copyableTypeSelectors = copyableTypeSelectors.Where(
            t => string.Equals(t.CopierName, WellKnownAlias, StringComparison.Ordinal))
            .ToArray();
        _options = options.Value;
    }

    #region IDeepCopier implementations

    /// <inheritdoc/>
    object? IDeepCopier.DeepCopy(object? input, CopyContext context)
    {
        if (input is null)
        {
            return null;
        }

        if (context.TryGetCopy(input, out object? result))
        {
            return result;
        }

        var type = input.GetType();
        if (type.GetCustomAttribute<ImmutableAttribute>() is not null)
        {
            result = input;
        }
        else
        {
            var bufferWriter = new BufferWriterBox<PooledBuffer>(new());
            try
            {
                MemoryPackSerializer.Serialize(type, bufferWriter, input, _options.SerializerOptions);

                var sequence = bufferWriter.Value.AsReadOnlySequence();
                result = MemoryPackSerializer.Deserialize(type, sequence, _options.SerializerOptions);
            }
            finally
            {
                bufferWriter.Value.Dispose();
            }
        }

        Debug.Assert(result is not null, "result != null");
        context.RecordCopy(input, result);

        return result;
    }

    #endregion

    #region IFieldCodec implementations

    /// <inheritdoc/>
    object? IFieldCodec.ReadValue<TInput>(ref Reader<TInput> reader, Field field)
    {
        if (field.IsReference)
        {
            return ReferenceCodec.ReadReference(ref reader, field.FieldType);
        }

        field.EnsureWireTypeTagDelimited();

        var placeholderReferenceId = ReferenceCodec.CreateRecordPlaceholder(reader.Session);
        object? result = null;
        Type? type = null;
        uint fieldId = 0;

        while (true)
        {
            var header = reader.ReadFieldHeader();
            if (header.IsEndBaseOrEndObject)
            {
                break;
            }

            fieldId += header.FieldIdDelta;
            switch (fieldId)
            {
                case 0:
                    ReferenceCodec.MarkValueField(reader.Session);
                    type = reader.Session.TypeCodec.ReadLengthPrefixed(ref reader);
                    break;

                case 1:
                    if (type is null)
                    {
                        ThrowTypeFieldMissing();
                    }

                    result = Deserialize(ref reader, type, _options.SerializerOptions);
                    break;

                default:
                    reader.ConsumeUnknownField(header);
                    break;
            }
        }

        ReferenceCodec.RecordObject(reader.Session, result, placeholderReferenceId);
        return result;

        static object? Deserialize(ref Reader<TInput> reader, Type type, MemoryPackSerializerOptions options)
        {
            ReferenceCodec.MarkValueField(reader.Session);
            var length = reader.ReadVarUInt32();

            var bufferWriter = new BufferWriterBox<PooledBuffer>(new());
            try
            {
                reader.ReadBytes(ref bufferWriter, (int)length);

                return MemoryPackSerializer.Deserialize(type, bufferWriter.Value.AsReadOnlySequence(), options);
            }
            finally
            {
                bufferWriter.Value.Dispose();
            }
        }
    }

    /// <inheritdoc/>
    void IFieldCodec.WriteField<TBufferWriter>(
        ref Writer<TBufferWriter> writer,
        uint fieldIdDelta,
        Type expectedType,
        object value)
    {
        if (ReferenceCodec.TryWriteReferenceField(ref writer, fieldIdDelta, expectedType, value))
        {
            return;
        }

        // The schema type when serializing the field is the type of the codec.
        writer.WriteFieldHeader(fieldIdDelta, expectedType, SelfType, WireType.TagDelimited);

        // Write the type name
        ReferenceCodec.MarkValueField(writer.Session);
        writer.WriteFieldHeaderExpected(0, WireType.LengthPrefixed);
        var type = value.GetType();
        writer.Session.TypeCodec.WriteLengthPrefixed(ref writer, type);

        var bufferWriter = new BufferWriterBox<PooledBuffer>(new());
        try
        {
            MemoryPackSerializer.Serialize(type, bufferWriter, value, _options.SerializerOptions);

            ReferenceCodec.MarkValueField(writer.Session);
            writer.WriteFieldHeaderExpected(1, WireType.LengthPrefixed);
            writer.WriteVarUInt32((uint)bufferWriter.Value.Length);
            bufferWriter.Value.CopyTo(ref writer);
        }
        finally
        {
            bufferWriter.Value.Dispose();
        }

        writer.WriteEndObject();
    }

    #endregion

    #region IGeneralizedCodec implementations

    /// <inheritdoc/>
    bool IGeneralizedCodec.IsSupportedType(Type type)
    {
        if (type == SelfType)
        {
            return true;
        }

        if (CommonCodecTypeFilter.IsAbstractOrFrameworkType(type))
        {
            return false;
        }

        if (Array.Exists(_serializableTypeSelectors, o => o.IsSupportedType(type)))
        {
            return true;
        }

        if (_options.IsSerializableType?.Invoke(type) is bool value)
        {
            return value;
        }

        return IsMemoryPackContract(type);
    }

    #endregion

    #region IGeneralizedCopier implementations

    /// <inheritdoc/>
    bool IGeneralizedCopier.IsSupportedType(Type type)
    {
        if (CommonCodecTypeFilter.IsAbstractOrFrameworkType(type))
        {
            return false;
        }

        if (Array.Exists(_copyableTypeSelectors, o => o.IsSupportedType(type)))
        {
            return true;
        }

        if (_options.IsCopyableType?.Invoke(type) is bool value)
        {
            return value;
        }

        return IsMemoryPackContract(type);
    }

    #endregion

    #region ITypeFilter implementations

    /// <inheritdoc/>
    bool? ITypeFilter.IsTypeAllowed(Type type)
    {
        return (((IGeneralizedCopier)this).IsSupportedType(type) || ((IGeneralizedCodec)this).IsSupportedType(type))
            ? true
            : null;
    }

    #endregion

}
