using Orleans;
using System;

namespace Grains;

[Immutable]
[GenerateSerializer]
[Alias("Grains.Depot")]
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

    [Id(0)]
    public DateTime CreationTime { get; set; }

    [Id(1)]
    public long Id { get; set; }

    [Id(2)]
    public string Name { get; set; }

    #endregion

}
