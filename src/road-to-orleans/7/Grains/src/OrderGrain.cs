using Interfaces;
using Orleans;
using Orleans.Runtime;
using System.Threading.Tasks;

namespace Grains;

public class OrderGrain : Grain, IOrderGrain
{
    private readonly IPersistentState<Order> _orders;

    public OrderGrain([PersistentState("Order")] IPersistentState<Order> persistentState)
    {
        _orders = persistentState;
    }

    #region IOrderGrain implementations

    public async Task CreateAsync(OrderInput order, GrainCancellationToken? token = null)
    {
        if (_orders.RecordExists)
        {
            return;
        }

        _orders.State = new Order(order.CreationTime, this.GetPrimaryKeyLong(), order.Number);

        await _orders.WriteStateAsync();
    }

    #endregion

}
