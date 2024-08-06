using Orleans;
using System.Threading.Tasks;

namespace Interfaces;

[Alias("Interfaces.IOrderGrain")]
public interface IOrderGrain : IGrainWithIntegerKey
{

    #region Methods

    /// <summary>
    /// Creates order.
    /// </summary>
    /// <param name="order">The order.</param>
    /// <param name="token">The token.</param>
    /// <returns></returns>
    [Alias("CreateAsync")]
    Task CreateAsync(OrderCreateInput order, GrainCancellationToken? token = null);

    /// <summary>
    /// Creates the order and stock asynchronous.
    /// </summary>
    /// <param name="order">The order.</param>
    /// <param name="stock">The stock.</param>
    /// <param name="token">The token.</param>
    /// <returns></returns>
    [Alias("CreateWithStockAsync")]
    [Transaction(TransactionOption.Create)]
    Task CreateWithStockAsync(OrderCreateInput order, StockCreateInput stock, GrainCancellationToken? token = null);

    /// <summary>
    /// Creates order.
    /// </summary>
    /// <param name="order">The order.</param>
    /// <param name="token">The token.</param>
    /// <returns></returns>
    [Alias("DeleteAsync")]
    Task DeleteAsync(OrderDeleteInput order, GrainCancellationToken? token = null);

    /// <summary>
    /// Creates order.
    /// </summary>
    /// <param name="order">The order.</param>
    /// <param name="token">The token.</param>
    /// <returns></returns>
    [Alias("UpdateAsync")]
    Task UpdateAsync(OrderUpdateInput order, GrainCancellationToken? token = null);

    #endregion

}
