using Orleans;
using System.Threading.Tasks;

namespace Interfaces;

[Alias("Interfaces.IDepotGrain")]
public interface IDepotGrain : IGrainWithIntegerKey
{

    #region Methods

    /// <summary>
    /// Creates the error with stock asynchronous.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns></returns>
    [Alias("CreateErrorWithStockAsync")]
    [Transaction(TransactionOption.Create)]
    Task CreateErrorWithStockAsync(GrainCancellationToken? token = null);

    /// <summary>
    /// Creates the with stock no state asynchronous.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns></returns>
    [Alias("CreateNoStateWithStockAsync")]
    [Transaction(TransactionOption.Create)]
    Task CreateNoStateWithStockAsync(GrainCancellationToken? token = null);

    /// <summary>
    /// Creates the depot and stock asynchronous.
    /// </summary>
    /// <param name="depot">The depot.</param>
    /// <param name="token">The token.</param>
    /// <returns></returns>
    [Alias("CreateWithStockAsync")]
    [Transaction(TransactionOption.Create)]
    Task CreateWithStockAsync(DepotCreateInput depot, GrainCancellationToken? token = null);

    /// <summary>
    /// Creates the with stock error asynchronous.
    /// </summary>
    /// <param name="depot">The depot.</param>
    /// <param name="token">The token.</param>
    /// <returns></returns>
    [Alias("CreateWithStockErrorAsync")]
    [Transaction(TransactionOption.Create)]
    Task CreateWithStockErrorAsync(DepotCreateInput depot, GrainCancellationToken? token = null);

    #endregion

}
