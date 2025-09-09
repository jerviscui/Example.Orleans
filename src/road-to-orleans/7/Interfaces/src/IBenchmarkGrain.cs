using Orleans;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Interfaces;

[Alias("Interfaces.IBenchmarkGrain")]
public interface IBenchmarkGrain : IGrainWithIntegerKey
{

    #region Methods

    /// <summary>
    /// Benchmark 1.
    /// </summary>
    /// <param name="order">The order.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    [Alias("Benchmark1ArrayAsync")]
    public Task<IReadOnlyList<IntClass1>> Benchmark1ArrayAsync(
        IReadOnlyList<IntClass1> order,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Benchmark 1.
    /// </summary>
    /// <param name="order">The order.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    [Alias("Benchmark1Async")]
    public Task<IntClass1> Benchmark1Async(IntClass1 order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Benchmark 2.
    /// </summary>
    /// <param name="order">The order.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    [Alias("Benchmark2ArrayAsync")]
    public Task<IReadOnlyList<IntClass2>> Benchmark2ArrayAsync(
        IReadOnlyList<IntClass2> order,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Benchmark 2.
    /// </summary>
    /// <param name="order">The order.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    [Alias("Benchmark2Async")]
    public Task<IntClass2> Benchmark2Async(IntClass2 order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Benchmark 3.
    /// </summary>
    /// <param name="order">The order.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    [Alias("Benchmark3ArrayAsync")]
    public Task<IReadOnlyList<IntClass3>> Benchmark3ArrayAsync(
        IReadOnlyList<IntClass3> order,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Benchmark 3.
    /// </summary>
    /// <param name="order">The order.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    [Alias("Benchmark3Async")]
    public Task<IntClass3> Benchmark3Async(IntClass3 order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Benchmark 1.
    /// </summary>
    /// <param name="order">The order.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    [Alias("BenchmarkStruct1ArrayAsync")]
    public Task<IntStruct1[]> BenchmarkStruct1ArrayAsync(
        IntStruct1[] order,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Benchmark 1.
    /// </summary>
    /// <param name="order">The order.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    [Alias("BenchmarkStruct1Async")]
    public Task<IntStruct1> BenchmarkStruct1Async(IntStruct1 order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Benchmark 2.
    /// </summary>
    /// <param name="order">The order.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    [Alias("BenchmarkStruct2ArrayAsync")]
    public Task<IntStruct2[]> BenchmarkStruct2ArrayAsync(
        IntStruct2[] order,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Benchmark 2.
    /// </summary>
    /// <param name="order">The order.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    [Alias("BenchmarkStruct2Async")]
    public Task<IntStruct2> BenchmarkStruct2Async(IntStruct2 order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Benchmark 3.
    /// </summary>
    /// <param name="order">The order.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    [Alias("BenchmarkStruct3ArrayAsync")]
    public Task<IntStruct3[]> BenchmarkStruct3ArrayAsync(
        IntStruct3[] order,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Benchmark 3.
    /// </summary>
    /// <param name="order">The order.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    [Alias("BenchmarkStruct3Async")]
    public Task<IntStruct3> BenchmarkStruct3Async(IntStruct3 order, CancellationToken cancellationToken = default);

    #endregion

}
