using System;

namespace Grains;

public class Order
{
    // https://github.com/dotnet/orleans/issues/9093
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Order()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

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

}
