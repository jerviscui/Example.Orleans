using Orleans;

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
    public int Count { get; set; }

    [Id(0)]
    public required string Goods { get; set; }

    #endregion

}
