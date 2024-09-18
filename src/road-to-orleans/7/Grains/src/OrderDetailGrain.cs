using Interfaces;
using Orleans;
using Orleans.Runtime;
using System.Threading.Tasks;

namespace Grains;

public class OrderDetailGrain : Grain, IOrderDetailGrain
{
    private readonly IPersistentState<OrderDetail> _orderDetail;

    public OrderDetailGrain([PersistentState("Order")] IPersistentState<OrderDetail> persistentState)
    {
        _orderDetail = persistentState;
    }

    #region IOrderDetailGrain implementations

    public async Task CreateAsync(OrderDetailInput detail, GrainCancellationToken? token = null)
    {
        if (_orderDetail.RecordExists)
        {
            return;
        }

        _orderDetail.State = new OrderDetail(detail.Goods, detail.Count);

        await _orderDetail.WriteStateAsync();
    }

    public Task CreateErrorAsync(OrderDetailInput detail, GrainCancellationToken? token = null)
    {
        throw new PersistenceException("OrderDetailGrain test");
    }

    #endregion

}
