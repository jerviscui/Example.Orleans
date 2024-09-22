using Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IClusterClient _clusterClient;
    private readonly ILogger<OrderController> _logger;

    public OrderController(ILogger<OrderController> logger, IClusterClient clusterClient)
    {
        _logger = logger;
        _clusterClient = clusterClient;
    }

    #region Methods

    [HttpPost("Create/{id}")]
    public async Task<IActionResult> CreateAsync(long id, OrderCreateInput order,
        CancellationToken cancellationToken = default)
    {
        var key = id;

        try
        {
            using var gcts = new GrainCancellationTokenSource();
            using var registration = gcts.RegisterTo(cancellationToken);

            var orderGrain = _clusterClient.GetGrain<IOrderGrain>(key);

            await orderGrain.CreateAsync(order, gcts.Token);
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

        return Ok($"Order created: {key}");
    }

    [HttpPost("CreateErrorWithDetail/{id}")]
    public async Task<IActionResult> CreateErrorWithDetailAsync(long id, OrderCreateWithDetailInput order,
        CancellationToken cancellationToken = default)
    {
        var key = id;

        try
        {
            using var gcts = new GrainCancellationTokenSource();
            using var registration = gcts.RegisterTo(cancellationToken);

            var orderGrain = _clusterClient.GetGrain<IOrderGrain>(key);

            // OrderDetail created, but Order not created
            await orderGrain.CreateErrorWithDetailAsync(order, gcts.Token);
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

        return Ok($"Order created: {key}");
    }

    [HttpPost("CreateWithDetail/{id}")]
    public async Task<IActionResult> CreateWithDetailAsync(long id, OrderCreateWithDetailInput order,
        CancellationToken cancellationToken = default)
    {
        var key = id;

        try
        {
            using var gcts = new GrainCancellationTokenSource();
            using var registration = gcts.RegisterTo(cancellationToken);

            var orderGrain = _clusterClient.GetGrain<IOrderGrain>(key);

            // OrderOrderDetail created, but OrderDetail not created
            await orderGrain.CreateWithDetailAsync(order, gcts.Token);
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

        return Ok($"Order created: {key}");
    }

    [HttpPost("CreateWithDetailError/{id}")]
    public async Task<IActionResult> CreateWithDetailErrorAsync(long id, OrderCreateWithDetailInput order,
        CancellationToken cancellationToken = default)
    {
        var key = id;

        try
        {
            using var gcts = new GrainCancellationTokenSource();
            using var registration = gcts.RegisterTo(cancellationToken);

            var orderGrain = _clusterClient.GetGrain<IOrderGrain>(key);

            await orderGrain.CreateWithDetailErrorAsync(order, gcts.Token);
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

        return Ok($"Order created: {key}");
    }

    [HttpDelete("Delete")]
    public async Task<IActionResult> DeleteAsync(OrderDeleteInput order, CancellationToken cancellationToken = default)
    {
        var key = order.Id;

        try
        {
            using var gcts = new GrainCancellationTokenSource();
            using var registration = gcts.RegisterTo(cancellationToken);

            var orderGrain = _clusterClient.GetGrain<IOrderGrain>(key);

            await orderGrain.DeleteAsync(order, gcts.Token);
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

        return Ok($"Order deleted: {key}");
    }

    [HttpPut("Update")]
    public async Task<IActionResult> UpdateAsync(OrderUpdateInput order, CancellationToken cancellationToken = default)
    {
        var key = order.Id;

        try
        {
            using var gcts = new GrainCancellationTokenSource();
            using var registration = gcts.RegisterTo(cancellationToken);

            var orderGrain = _clusterClient.GetGrain<IOrderGrain>(key);

            await orderGrain.UpdateAsync(order, gcts.Token);
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

        return Ok($"Order updated: {key}");
    }

    #endregion

}
