using Orleans;

namespace SiloHost2;

[Immutable]
[GenerateSerializer]
[Alias("SiloHost2.Stock")]
public class Stock
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Stock()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    public Stock(long id, string goods, int count)
    {
        Count = count;
        Goods = goods;
        Id = id;
    }

    #region Properties

    [Id(0)]
    public int Count { get; set; }

    [Id(1)]
    public string Goods { get; set; }

    [Id(2)]
    public long Id { get; set; }

    #endregion

}
