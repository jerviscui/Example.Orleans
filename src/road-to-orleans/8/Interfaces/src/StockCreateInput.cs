using Orleans;
using System.ComponentModel.DataAnnotations;

namespace Interfaces;

[Immutable]
[GenerateSerializer]
[Alias("Interfaces.StockCreateInput")]
public class StockCreateInput
{
    public StockCreateInput(string goods, int count)
    {
        Count = count;
        Goods = goods;
    }

    #region Properties

    [Id(1)]
    [Required]
    public int Count { get; set; }

    [Id(0)]
    [Required]
    public string Goods { get; set; }

    #endregion

}
