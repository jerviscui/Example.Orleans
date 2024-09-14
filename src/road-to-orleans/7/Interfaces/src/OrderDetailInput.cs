using Orleans;
using System.ComponentModel.DataAnnotations;

namespace Interfaces;

[Immutable]
[GenerateSerializer]
[Alias("Interfaces.OrderDetailInput")]
public class OrderDetailInput
{
    public OrderDetailInput(string goods, int count)
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
