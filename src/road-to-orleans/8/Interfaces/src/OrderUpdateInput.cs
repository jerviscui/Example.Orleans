using Orleans;
using System;
using System.Text.Json.Serialization;

namespace Interfaces;

[JsonSerializable(typeof(OrderUpdateInput))]
public partial class OrderUpdateInputJsonContext : JsonSerializerContext
{
}

[Immutable]
public class OrderUpdateInput
{
    // [JsonConstructor]
    public OrderUpdateInput(long id, DateTime? creationTime = null, string? number = null)
    {
        Id = id;
        CreationTime = creationTime;
        Number = number;
    }

    #region Properties

    public DateTime? CreationTime { get; set; }

    public long Id { get; set; }

    public string? Number { get; set; }

    #endregion

}
