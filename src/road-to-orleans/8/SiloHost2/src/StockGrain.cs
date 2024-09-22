using Interfaces;
using Orleans;
using Orleans.Transactions.Abstractions;
using System;
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
        try
        {
            await _stockState.PerformUpdate(o =>
            {
                o.Id = this.GetPrimaryKeyLong();
                o.Goods = stock.Goods;
                o.Count = stock.Count;
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public Task CreateErrorAsync(StockCreateInput stock, GrainCancellationToken? token = null)
    {
        throw new MyTransactionException("Stock test.");
    }

    #endregion

}
