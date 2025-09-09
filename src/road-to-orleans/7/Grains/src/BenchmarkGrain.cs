using Interfaces;
using Orleans;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Grains;

public class BenchmarkGrain : Grain, IBenchmarkGrain
{

    #region IBenchmarkGrain implementations

    public Task<IReadOnlyList<IntClass1>> Benchmark1ArrayAsync(
        IReadOnlyList<IntClass1> order,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(order);
    }

    public Task<IntClass1> Benchmark1Async(IntClass1 order, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(order);
    }

    public Task<IReadOnlyList<IntClass2>> Benchmark2ArrayAsync(
        IReadOnlyList<IntClass2> order,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(order);
    }

    public Task<IntClass2> Benchmark2Async(IntClass2 order, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(order);
    }

    public Task<IReadOnlyList<IntClass3>> Benchmark3ArrayAsync(
        IReadOnlyList<IntClass3> order,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(order);
    }

    public Task<IntClass3> Benchmark3Async(IntClass3 order, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(order);
    }

    public Task<IntStruct1[]> BenchmarkStruct1ArrayAsync(
        IntStruct1[] order,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(order);
    }

    public Task<IntStruct1> BenchmarkStruct1Async(IntStruct1 order, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(order);
    }

    public Task<IntStruct2[]> BenchmarkStruct2ArrayAsync(
        IntStruct2[] order,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(order);
    }

    public Task<IntStruct2> BenchmarkStruct2Async(IntStruct2 order, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(order);
    }

    public Task<IntStruct3[]> BenchmarkStruct3ArrayAsync(
        IntStruct3[] order,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(order);
    }

    public Task<IntStruct3> BenchmarkStruct3Async(IntStruct3 order, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(order);
    }

    #endregion

}
