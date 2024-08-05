using Interfaces;
using Orleans;
using Orleans.Runtime;
using System;
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

    public async Task CreateAsync(OrderCreateInput order, GrainCancellationToken? token = null)
    {
        if (_orders.RecordExists)
        {
            return;
        }

        _orders.State = new Order(order.CreationTime, this.GetPrimaryKeyLong(), order.Number);

        await _orders.WriteStateAsync();
    }

    public async Task DeleteAsync(OrderDeleteInput order, GrainCancellationToken? token = null)
    {
        if (_orders.RecordExists)
        {
            await _orders.ClearStateAsync();
        }
    }

    public async Task UpdateAsync(OrderUpdateInput order, GrainCancellationToken? token = null)
    {
        if (_orders.RecordExists is false)
        {
            return;
        }

        if (order.Number is not null)
        {
            _orders.State.Number = order.Number;
        }
        if (order.CreationTime is not null)
        {
            _orders.State.CreationTime = (DateTime)order.CreationTime;
        }

        await _orders.WriteStateAsync();
    }

    #endregion

}
