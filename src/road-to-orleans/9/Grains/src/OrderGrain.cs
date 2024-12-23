using Interfaces;
using Orleans;
using Orleans.Runtime;
using System.Threading.Tasks;

namespace Grains;

public class OrderGrain : Grain, IOrderGrain
{
    private readonly IPersistentState<Order> _orderState;

    public OrderGrain([PersistentState("Order")] IPersistentState<Order> persistentState)
    {
        _orderState = persistentState;
    }

    #region IOrderGrain implementations

    public async Task CreateAsync(OrderCreateInput order, GrainCancellationToken? token = null)
    {
        if (_orderState.RecordExists)
        {
            return;
        }

        _orderState.State = new Order(order.CreationTime, this.GetPrimaryKeyLong(), order.Number);

        await _orderState.WriteStateAsync();
    }

    #endregion

}
