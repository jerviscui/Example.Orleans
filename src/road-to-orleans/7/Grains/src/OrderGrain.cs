using Interfaces;
using Orleans;
using Orleans.Runtime;
using System;
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

    public Task CreateErrorWithDetailAsync(OrderCreateWithDetailInput order, GrainCancellationToken? token = null)
    {
        throw new NotImplementedException();
    }

    public Task CreateWithDetailAsync(OrderCreateWithDetailInput order, GrainCancellationToken? token = null)
    {
        throw new NotImplementedException();
    }

    public Task CreateWithDetailErrorAsync(OrderCreateWithDetailInput order, GrainCancellationToken? token = null)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(OrderDeleteInput order, GrainCancellationToken? token = null)
    {
        if (_orderState.RecordExists)
        {
            await _orderState.ClearStateAsync();
        }
    }

    public async Task UpdateAsync(OrderUpdateInput order, GrainCancellationToken? token = null)
    {
        if (!_orderState.RecordExists)
        {
            return;
        }

        if (order.Number is not null)
        {
            _orderState.State.Number = order.Number;
        }
        if (order.CreationTime is not null)
        {
            _orderState.State.CreationTime = (DateTime)order.CreationTime;
        }

        await _orderState.WriteStateAsync();
    }

    #endregion

}
