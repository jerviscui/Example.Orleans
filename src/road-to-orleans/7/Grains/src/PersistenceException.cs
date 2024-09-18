using Orleans;
using System;
using System.Runtime.Serialization;

namespace Grains;

[Serializable]
[GenerateSerializer]
[Alias("Grains.PersistenceException")]
public class PersistenceException : Exception
{
    protected PersistenceException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public PersistenceException()
    {
    }

    public PersistenceException(string message) : base(message)
    {
    }

    public PersistenceException(string message, Exception inner) : base(message, inner)
    {
    }
}
