using Interfaces;
using MemoryPack;
using Microsoft.AspNetCore.Mvc;
using Orleans.Serialization.Buffers;
using System.Diagnostics;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BenchmarkController : ControllerBase
{
    private readonly IClusterClient _clusterClient;
    private readonly ILogger<BenchmarkController> _logger;

    public BenchmarkController(ILogger<BenchmarkController> logger, IClusterClient clusterClient)
    {
        _logger = logger;
        _clusterClient = clusterClient;
    }

    #region Methods

    [HttpPost("Benchmark1Array/{id}")]
    public async Task<IActionResult> Benchmark1ArrayAsync(
        long id,
        int count = 1,
        CancellationToken cancellationToken = default)
    {
        var key = id;

        var start = Stopwatch.GetTimestamp();

        try
        {
            var order = Enumerable.Repeat(IntClass1.Create(), 100).ToArray();
            var orderGrain = _clusterClient.GetGrain<IBenchmarkGrain>(key);

            for (var i = 0; i < count; i++)
            {
                _ = await orderGrain.Benchmark1ArrayAsync(order, cancellationToken);
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.GrainCanceled(ex.Message);

            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.GrainError(ex.Message);

            return BadRequest(ex.Message);
        }

        return Ok($"ElapsedTime: {Stopwatch.GetElapsedTime(start, Stopwatch.GetTimestamp())}");
    }

    [HttpPost("Benchmark1/{id}")]
    public async Task<IActionResult> Benchmark1Async(
        long id,
        int count = 1,
        CancellationToken cancellationToken = default)
    {
        var key = id;

        var start = Stopwatch.GetTimestamp();

        try
        {
            var order = IntClass1.Create();
            var orderGrain = _clusterClient.GetGrain<IBenchmarkGrain>(key);

            for (var i = 0; i < count; i++)
            {
                _ = await orderGrain.Benchmark1Async(order, cancellationToken);
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.GrainCanceled(ex.Message);

            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.GrainError(ex.Message);

            return BadRequest(ex.Message);
        }

        return Ok($"ElapsedTime: {Stopwatch.GetElapsedTime(start, Stopwatch.GetTimestamp())}");
    }

    [HttpPost("Benchmark2Array/{id}")]
    public async Task<IActionResult> Benchmark2ArrayAsync(
        long id,
        int count = 1,
        CancellationToken cancellationToken = default)
    {
        var key = id;

        var start = Stopwatch.GetTimestamp();

        try
        {
            var order = Enumerable.Repeat(IntClass2.Create(), 100).ToArray();
            var orderGrain = _clusterClient.GetGrain<IBenchmarkGrain>(key);

            for (var i = 0; i < count; i++)
            {
                _ = await orderGrain.Benchmark2ArrayAsync(order, cancellationToken);
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.GrainCanceled(ex.Message);

            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.GrainError(ex.Message);

            return BadRequest(ex.Message);
        }

        return Ok($"ElapsedTime: {Stopwatch.GetElapsedTime(start, Stopwatch.GetTimestamp())}");
    }

    [HttpPost("Benchmark2/{id}")]
    public async Task<IActionResult> Benchmark2Async(
        long id,
        int count = 1,
        CancellationToken cancellationToken = default)
    {
        var key = id;

        var start = Stopwatch.GetTimestamp();

        try
        {
            var order = IntClass2.Create();
            var orderGrain = _clusterClient.GetGrain<IBenchmarkGrain>(key);

            for (var i = 0; i < count; i++)
            {
                _ = await orderGrain.Benchmark2Async(order, cancellationToken);
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.GrainCanceled(ex.Message);

            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.GrainError(ex.Message);

            return BadRequest(ex.Message);
        }

        return Ok($"ElapsedTime: {Stopwatch.GetElapsedTime(start, Stopwatch.GetTimestamp())}");
    }

    [HttpPost("Benchmark3Array/{id}")]
    public async Task<IActionResult> Benchmark3ArrayAsync(
        long id,
        int count = 1,
        CancellationToken cancellationToken = default)
    {
        var key = id;

        var start = Stopwatch.GetTimestamp();

        try
        {
            var order = Enumerable.Repeat(IntClass3.Create(), 100).ToArray();
            var orderGrain = _clusterClient.GetGrain<IBenchmarkGrain>(key);

            for (var i = 0; i < count; i++)
            {
                _ = await orderGrain.Benchmark3ArrayAsync(order, cancellationToken);
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.GrainCanceled(ex.Message);

            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.GrainError(ex.Message);

            return BadRequest(ex.Message);
        }

        return Ok($"ElapsedTime: {Stopwatch.GetElapsedTime(start, Stopwatch.GetTimestamp())}");
    }

    [HttpPost("Benchmark3/{id}")]
    public async Task<IActionResult> Benchmark3Async(
        long id,
        int count = 1,
        CancellationToken cancellationToken = default)
    {
        var key = id;

        var start = Stopwatch.GetTimestamp();

        try
        {
            var order = IntClass3.Create();
            var orderGrain = _clusterClient.GetGrain<IBenchmarkGrain>(key);

            for (var i = 0; i < count; i++)
            {
                _ = await orderGrain.Benchmark3Async(order, cancellationToken);
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.GrainCanceled(ex.Message);

            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.GrainError(ex.Message);

            return BadRequest(ex.Message);
        }

        return Ok($"ElapsedTime: {Stopwatch.GetElapsedTime(start, Stopwatch.GetTimestamp())}");
    }

    [HttpPost("BenchmarkNoGrain/{id}")]
    public async Task<IActionResult> BenchmarkNoGrainAsync(
        long id,
        int count = 1,
        CancellationToken cancellationToken = default)
    {
        var key = id;

        var start = Stopwatch.GetTimestamp();

        try
        {
            var order = IntClass1.Create();

            for (var i = 0; i < count; i++)
            {
                var buffer = new PooledBuffer();
                MemoryPackSerializer.Serialize(buffer, order);
                _ = MemoryPackSerializer.Deserialize<IntClass1>(buffer.AsReadOnlySequence());
                buffer.Dispose();

                var buffer2 = new PooledBuffer();
                MemoryPackSerializer.Serialize(buffer2, order);
                _ = MemoryPackSerializer.Deserialize<IntClass1>(buffer2.AsReadOnlySequence());
                buffer.Dispose();

                await Task.Yield();
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.GrainCanceled(ex.Message);

            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.GrainError(ex.Message);

            return BadRequest(ex.Message);
        }

        return Ok($"ElapsedTime: {Stopwatch.GetElapsedTime(start, Stopwatch.GetTimestamp())}");
    }

    [HttpPost("BenchmarkStruct1Array/{id}")]
    public async Task<IActionResult> BenchmarkStruct1ArrayAsync(
        long id,
        int count = 1,
        CancellationToken cancellationToken = default)
    {
        var key = id;

        var start = Stopwatch.GetTimestamp();

        try
        {
            var order = Enumerable.Repeat(IntStruct1.Create(), 100).ToArray();
            var orderGrain = _clusterClient.GetGrain<IBenchmarkGrain>(key);

            for (var i = 0; i < count; i++)
            {
                _ = await orderGrain.BenchmarkStruct1ArrayAsync(order, cancellationToken);
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.GrainCanceled(ex.Message);

            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.GrainError(ex.Message);

            return BadRequest(ex.Message);
        }

        return Ok($"ElapsedTime: {Stopwatch.GetElapsedTime(start, Stopwatch.GetTimestamp())}");
    }

    [HttpPost("BenchmarkStruct1/{id}")]
    public async Task<IActionResult> BenchmarkStruct1Async(
        long id,
        int count = 1,
        CancellationToken cancellationToken = default)
    {
        var key = id;

        var start = Stopwatch.GetTimestamp();

        try
        {
            var order = IntStruct1.Create();
            var orderGrain = _clusterClient.GetGrain<IBenchmarkGrain>(key);

            for (var i = 0; i < count; i++)
            {
                _ = await orderGrain.BenchmarkStruct1Async(order, cancellationToken);
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.GrainCanceled(ex.Message);

            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.GrainError(ex.Message);

            return BadRequest(ex.Message);
        }

        return Ok($"ElapsedTime: {Stopwatch.GetElapsedTime(start, Stopwatch.GetTimestamp())}");
    }

    [HttpPost("BenchmarkStruct2Array/{id}")]
    public async Task<IActionResult> BenchmarkStruct2ArrayAsync(
        long id,
        int count = 1,
        CancellationToken cancellationToken = default)
    {
        var key = id;

        var start = Stopwatch.GetTimestamp();

        try
        {
            var order = Enumerable.Repeat(IntStruct2.Create(), 100).ToArray();
            var orderGrain = _clusterClient.GetGrain<IBenchmarkGrain>(key);

            for (var i = 0; i < count; i++)
            {
                _ = await orderGrain.BenchmarkStruct2ArrayAsync(order, cancellationToken);
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.GrainCanceled(ex.Message);

            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.GrainError(ex.Message);

            return BadRequest(ex.Message);
        }

        return Ok($"ElapsedTime: {Stopwatch.GetElapsedTime(start, Stopwatch.GetTimestamp())}");
    }

    [HttpPost("BenchmarkStruct2/{id}")]
    public async Task<IActionResult> BenchmarkStruct2Async(
        long id,
        int count = 1,
        CancellationToken cancellationToken = default)
    {
        var key = id;

        var start = Stopwatch.GetTimestamp();

        try
        {
            var order = IntStruct2.Create();
            var orderGrain = _clusterClient.GetGrain<IBenchmarkGrain>(key);

            for (var i = 0; i < count; i++)
            {
                _ = await orderGrain.BenchmarkStruct2Async(order, cancellationToken);
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.GrainCanceled(ex.Message);

            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.GrainError(ex.Message);

            return BadRequest(ex.Message);
        }

        return Ok($"ElapsedTime: {Stopwatch.GetElapsedTime(start, Stopwatch.GetTimestamp())}");
    }

    [HttpPost("BenchmarkStruct3Array/{id}")]
    public async Task<IActionResult> BenchmarkStruct3ArrayAsync(
        long id,
        int count = 1,
        CancellationToken cancellationToken = default)
    {
        var key = id;

        var start = Stopwatch.GetTimestamp();

        try
        {
            var order = Enumerable.Repeat(IntStruct3.Create(), 100).ToArray();
            var orderGrain = _clusterClient.GetGrain<IBenchmarkGrain>(key);

            for (var i = 0; i < count; i++)
            {
                _ = await orderGrain.BenchmarkStruct3ArrayAsync(order, cancellationToken);
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.GrainCanceled(ex.Message);

            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.GrainError(ex.Message);

            return BadRequest(ex.Message);
        }

        return Ok($"ElapsedTime: {Stopwatch.GetElapsedTime(start, Stopwatch.GetTimestamp())}");
    }

    [HttpPost("BenchmarkStruct3/{id}")]
    public async Task<IActionResult> BenchmarkStruct3Async(
        long id,
        int count = 1,
        CancellationToken cancellationToken = default)
    {
        var key = id;

        var start = Stopwatch.GetTimestamp();

        try
        {
            var order = IntStruct3.Create();
            var orderGrain = _clusterClient.GetGrain<IBenchmarkGrain>(key);

            for (var i = 0; i < count; i++)
            {
                _ = await orderGrain.BenchmarkStruct3Async(order, cancellationToken);
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.GrainCanceled(ex.Message);

            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.GrainError(ex.Message);

            return BadRequest(ex.Message);
        }

        return Ok($"ElapsedTime: {Stopwatch.GetElapsedTime(start, Stopwatch.GetTimestamp())}");
    }

    #endregion

}
