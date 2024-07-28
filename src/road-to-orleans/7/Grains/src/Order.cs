using System;

namespace Grains;

public class Order
{
    public Order(DateTime creationTime, long id, string number)
    {
        CreationTime = creationTime;
        Id = id;
        Number = number;
    }

    #region Properties

    public DateTime CreationTime { get; set; }

    public long Id { get; set; }

    public string Number { get; set; }

    #endregion

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    internal Order()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }
}
