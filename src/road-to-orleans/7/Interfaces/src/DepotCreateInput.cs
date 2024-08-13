using Orleans;
using System;

namespace Interfaces;

[Immutable]
[GenerateSerializer]
[Alias("Interfaces.DepotCreateInput")]
public class DepotCreateInput
{
    public DepotCreateInput(string name, DateTime creationTime, StockCreateInput stockCreateInput)
    {
        Name = name;
        CreationTime = creationTime;
        StockCreateInput = stockCreateInput;
    }

    #region Properties

    [Id(0)]
    public DateTime CreationTime { get; set; }

    [Id(1)]
    public string Name { get; set; }

    [Id(2)]
    public StockCreateInput StockCreateInput { get; set; }

    #endregion

}
