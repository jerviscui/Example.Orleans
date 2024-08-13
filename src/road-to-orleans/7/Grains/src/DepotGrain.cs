using Interfaces;
using Orleans;
using Orleans.Transactions.Abstractions;
using System.Threading.Tasks;

namespace Grains;

public class DepotGrain : Grain, IDepotGrain
{
    private readonly IGrainFactory _factory;
    private readonly ITransactionalState<Depot> _depotState;

    public DepotGrain(IGrainFactory factory,
        [TransactionalState("Depot", "AzureTable")] ITransactionalState<Depot> depots)
    {
        _factory = factory;
        _depotState = depots;
    }

    #region IDepotGrain implementations

    public async Task CreateWithStockAsync(DepotCreateInput depot, GrainCancellationToken? token = null)
    {
        var data = await _depotState.PerformRead((o) => o);

        if (data is null)
        {
            return;
        }

        var stockGrain = _factory.GetGrain<IStockGrain>(this.GetPrimaryKeyLong());
        await stockGrain.CreateAsync(depot.StockCreateInput, token);

        await _depotState.PerformUpdate((o) =>
        {
            o = new Depot(depot.CreationTime, this.GetPrimaryKeyLong(), depot.Name);
        });
    }

    #endregion

}
