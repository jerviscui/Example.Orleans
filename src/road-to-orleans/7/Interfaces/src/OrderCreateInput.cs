using MemoryPack;
using Orleans;
using System;

namespace Interfaces;

[Immutable]
[MemoryPackable]
public partial class OrderCreateInput
{
    public OrderCreateInput(string number, DateTime creationTime)
    {
        Number = number;
        CreationTime = creationTime;
    }

    #region Properties

    [MemoryPackOrder(0)]
    public DateTime CreationTime { get; set; }

    [MemoryPackOrder(1)]
    public string Number { get; set; }

    #endregion

}
