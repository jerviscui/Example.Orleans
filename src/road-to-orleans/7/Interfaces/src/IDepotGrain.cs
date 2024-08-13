using Orleans;
using System.Threading.Tasks;

namespace Interfaces;

[Alias("Interfaces.IDepot")]
public interface IDepotGrain : IGrainWithIntegerKey
{

    #region Methods

    /// <summary>
    /// Creates the depot and stock asynchronous.
    /// </summary>
    /// <param name="depot">The depot.</param>
    /// <param name="token">The token.</param>
    /// <returns></returns>
    [Alias("CreateWithStockAsync")]
    [Transaction(TransactionOption.Create)]
    Task CreateWithStockAsync(DepotCreateInput depot, GrainCancellationToken? token = null);

    #endregion

}
