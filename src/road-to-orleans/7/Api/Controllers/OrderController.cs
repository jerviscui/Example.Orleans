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
    public async Task<IActionResult> CreateAsync(long id, OrderInput order,
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

    #endregion

}
