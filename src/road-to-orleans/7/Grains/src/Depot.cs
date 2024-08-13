using System;

namespace Grains;

public class Depot
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Depot()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    public Depot(DateTime creationTime, long id, string name)
    {
        CreationTime = creationTime;
        Id = id;
        Name = name;
    }

    #region Properties

    public DateTime CreationTime { get; set; }

    public long Id { get; set; }

    public string Name { get; set; }

    #endregion

}
