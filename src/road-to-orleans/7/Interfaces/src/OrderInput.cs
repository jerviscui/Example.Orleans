using Orleans;
using System;

namespace Interfaces;

[GenerateSerializer]
// [Immutable]
[Alias("Interfaces.OrderInput")]
public class OrderInput
{
    public OrderInput(string number, DateTime creationTime)
    {
        Number = number;
        CreationTime = creationTime;
    }

    #region Properties

    [Id(0)]
    public DateTime CreationTime { get; set; }

    [Id(1)]
    public required string Number { get; set; }

    #endregion

}
