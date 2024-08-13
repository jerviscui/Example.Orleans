using Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DepotController : ControllerBase
{
    private readonly IClusterClient _clusterClient;
    private readonly ILogger<DepotController> _logger;

    public DepotController(ILogger<DepotController> logger, IClusterClient clusterClient)
    {
        _logger = logger;
        _clusterClient = clusterClient;
    }

    #region Methods

    [HttpPost("CreateWithStock/{id}")]
    public async Task<IActionResult> CreateWithStockAsync(long id, DepotCreateInput depot,
        CancellationToken cancellationToken = default)
    {
        var key = id;

        try
        {
            using var gcts = new GrainCancellationTokenSource();
            using var registration = gcts.RegisterTo(cancellationToken);

            var depotGrain = _clusterClient.GetGrain<IDepotGrain>(key);

            await depotGrain.CreateWithStockAsync(depot, gcts.Token);
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

        return Ok($"Depot created: {key}");
    }

    #endregion

}
