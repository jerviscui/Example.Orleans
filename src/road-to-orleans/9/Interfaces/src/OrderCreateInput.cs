using Orleans;
using System;

namespace Interfaces;

[Immutable]
[GenerateSerializer]
[Alias("Interfaces.OrderInput")]
public class OrderCreateInput
{
    public OrderCreateInput(string number, DateTime creationTime)
    {
        Number = number;
        CreationTime = creationTime;
    }

    #region Properties

    [Id(0)]
    public DateTime CreationTime { get; set; }

    [Id(1)]
    public string Number { get; set; }

    #endregion

}
