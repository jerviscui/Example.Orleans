using Orleans;
using System.Threading.Tasks;

namespace Interfaces;

[Alias("Interfaces.IStockGrain")]
public interface IStockGrain : IGrainWithIntegerKey
{

    #region Methods

    /// <summary>
    /// Creates the stock.
    /// </summary>
    /// <param name="order">The order.</param>
    /// <param name="token">The token.</param>
    /// <returns></returns>
    [Alias("CreateAsync")]
    [Transaction(TransactionOption.CreateOrJoin)]
    Task CreateAsync(StockCreateInput stock, GrainCancellationToken? token = null);

    #endregion

}
