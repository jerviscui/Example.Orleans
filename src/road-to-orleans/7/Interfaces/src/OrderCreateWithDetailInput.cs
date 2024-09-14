using Orleans;
using System;

namespace Interfaces;

[Immutable]
[GenerateSerializer]
[Alias("Interfaces.OrderCreateWithDetailInput")]
public class OrderCreateWithDetailInput
{
    public OrderCreateWithDetailInput(string number, DateTime creationTime, OrderDetailInput detailInput)
    {
        CreationTime = creationTime;
        DetailInput = detailInput;
        Number = number;
    }

    #region Properties

    [Id(0)]
    public DateTime CreationTime { get; set; }

    [Id(2)]
    public OrderDetailInput DetailInput { get; set; }

    [Id(1)]
    public string Number { get; set; }

    #endregion

}
