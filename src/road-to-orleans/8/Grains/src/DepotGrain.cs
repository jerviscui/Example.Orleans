using Interfaces;
using Orleans;
using Orleans.Transactions.Abstractions;
using System;
using System.Threading.Tasks;

namespace Grains;

public class DepotGrain : Grain, IDepotGrain
{
    private readonly ITransactionalState<Depot> _depotState;

    public DepotGrain([TransactionalState("Depot", "AzureTable")] ITransactionalState<Depot> depots)
    {
        _depotState = depots;
    }

    #region IDepotGrain implementations

    public async Task CreateErrorWithStockAsync(GrainCancellationToken? token = null)
    {
        var stockGrain = GrainFactory.GetGrain<IStockGrain>(this.GetPrimaryKeyLong());
        await stockGrain.CreateAsync(new StockCreateInput("no state", 0), token);

        throw new MyTransactionException("Depot test.");
    }

    public async Task CreateNoStateWithStockAsync(GrainCancellationToken? token = null)
    {
        var stockGrain = GrainFactory.GetGrain<IStockGrain>(this.GetPrimaryKeyLong());
        await stockGrain.CreateAsync(new StockCreateInput("no state", 0), token);
    }

    public async Task CreateWithStockAsync(DepotCreateInput depot, GrainCancellationToken? token = null)
    {
        try
        {
            var data = await _depotState.PerformRead((o) => o);

            if (data.Id != 0)
            {
                return;
            }

            var stockGrain = GrainFactory.GetGrain<IStockGrain>(this.GetPrimaryKeyLong());
            await stockGrain.CreateAsync(depot.StockCreateInput, token);

            await _depotState.PerformUpdate((o) =>
            {
                o.Id = this.GetPrimaryKeyLong();
                o.Name = depot.Name;
                o.CreationTime = depot.CreationTime;
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task CreateWithStockErrorAsync(DepotCreateInput depot, GrainCancellationToken? token = null)
    {
        var data = await _depotState.PerformRead((o) => o);

        if (data.Id != 0)
        {
            return;
        }

        var stockGrain = GrainFactory.GetGrain<IStockGrain>(this.GetPrimaryKeyLong());
        await stockGrain.CreateErrorAsync(depot.StockCreateInput, token);

        await _depotState.PerformUpdate((o) =>
        {
            o.Id = this.GetPrimaryKeyLong();
            o.Name = depot.Name;
            o.CreationTime = depot.CreationTime;
        });
    }

    public async Task CreateWithTwoStockErrorAsync(DepotCreateInput depot, GrainCancellationToken? token = null)
    {
        var data = await _depotState.PerformRead((o) => o);

        if (data.Id != 0)
        {
            return;
        }

        var stockGrain1 = GrainFactory.GetGrain<IStockGrain>(this.GetPrimaryKeyLong());
        await stockGrain1.CreateAsync(depot.StockCreateInput, token);

        var stockGrain2 = GrainFactory.GetGrain<IStockGrain>(this.GetPrimaryKeyLong() + 1);
        await stockGrain2.CreateErrorAsync(depot.StockCreateInput, token);

        await _depotState.PerformUpdate((o) =>
        {
            o.Id = this.GetPrimaryKeyLong();
            o.Name = depot.Name;
            o.CreationTime = depot.CreationTime;
        });
    }

    #endregion

}
