using MessagePack;
using Orleans;

namespace Interfaces;

[Immutable]
[MessagePackObject]
public class OrderDeleteInput
{
    public OrderDeleteInput(long id, string number)
    {
        Id = id;
        Number = number;
    }

    #region Properties

    [Key(0)]
    public long Id { get; set; }

    [Key(1)]
    public string Number { get; set; }

    #endregion

}
