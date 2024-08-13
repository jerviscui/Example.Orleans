using Interfaces;
using Orleans;
using Orleans.Transactions.Abstractions;
using System.Threading.Tasks;

namespace SiloHost2;

public class StockGrain : Grain, IStockGrain
{
    private readonly ITransactionalState<Stock> _stockState;

    public StockGrain([TransactionalState("Stock", "AzureTable")] ITransactionalState<Stock> state)
    {
        _stockState = state;
    }

    #region IStockGrain implementations

    public async Task CreateAsync(StockCreateInput stock, GrainCancellationToken? token = null)
    {
        await _stockState.PerformUpdate(o =>
        {
            o = new Stock(this.GetPrimaryKeyLong(), stock.Goods, stock.Count);
        });
    }

    #endregion

}
