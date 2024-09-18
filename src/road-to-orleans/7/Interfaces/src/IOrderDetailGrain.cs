using Orleans;
using System.Threading.Tasks;

namespace Interfaces;

[Alias("Interfaces.IOrderDetailGrain")]
public interface IOrderDetailGrain : IGrainWithIntegerKey
{

    #region Methods

    /// <summary>
    /// Creates the detail.
    /// </summary>
    /// <param name="detail">The detail.</param>
    /// <param name="token">The token.</param>
    /// <returns></returns>
    [Alias("CreateAsync")]
    Task CreateAsync(OrderDetailInput detail, GrainCancellationToken? token = null);

    /// <summary>
    /// Creates the detail.
    /// </summary>
    /// <param name="detail">The detail.</param>
    /// <param name="token">The token.</param>
    /// <returns></returns>
    [Alias("CreateErrorAsync")]
    Task CreateErrorAsync(OrderDetailInput detail, GrainCancellationToken? token = null);

    #endregion

}
