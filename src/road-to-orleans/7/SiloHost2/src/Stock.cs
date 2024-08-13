namespace SiloHost2;

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

    public int Count { get; set; }

    public string Goods { get; set; }

    public long Id { get; set; }

    #endregion

}
