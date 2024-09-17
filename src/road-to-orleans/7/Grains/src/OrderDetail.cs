namespace Grains;

public class OrderDetail
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public OrderDetail()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    public OrderDetail(string goods, int count)
    {
        Count = count;
        Goods = goods;
    }

    #region Properties

    public int Count { get; set; }

    public string Goods { get; set; }

    #endregion

}
