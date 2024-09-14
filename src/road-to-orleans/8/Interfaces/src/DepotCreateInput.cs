using Orleans;
using System;
using System.ComponentModel.DataAnnotations;

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
    [Required]
    public DateTime CreationTime { get; set; }

    [Id(1)]
    [Required]
    public string Name { get; set; }

    [Id(2)]
    [Required]
    public StockCreateInput StockCreateInput { get; set; }

    #endregion

}
