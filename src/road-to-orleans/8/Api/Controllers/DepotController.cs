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

    [HttpPost("CreateErrorWithStock/{id}")]
    public async Task<IActionResult> CreateErrorWithStockAsync(long id, CancellationToken cancellationToken = default)
    {
        var key = id;

        try
        {
            using var gcts = new GrainCancellationTokenSource();
            using var registration = gcts.RegisterTo(cancellationToken);

            var depotGrain = _clusterClient.GetGrain<IDepotGrain>(key);

            // Depot not save KeyEntity & StateEntity
            // Stock not save KeyEntity & StateEntity
            await depotGrain.CreateErrorWithStockAsync(gcts.Token);
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

    [HttpPost("CreateNoStateWithStock/{id}")]
    public async Task<IActionResult> CreateNoStateWithStockAsync(long id, CancellationToken cancellationToken = default)
    {
        var key = id;

        try
        {
            using var gcts = new GrainCancellationTokenSource();
            using var registration = gcts.RegisterTo(cancellationToken);

            var depotGrain = _clusterClient.GetGrain<IDepotGrain>(key);

            // Depot not save KeyEntity & StateEntity
            // Stock save KeyEntity & StateEntity
            await depotGrain.CreateNoStateWithStockAsync(gcts.Token);
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

            // Depot save KeyEntity & StateEntity
            // Stock save KeyEntity & StateEntity
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

    [HttpPost("CreateWithStockError/{id}")]
    public async Task<IActionResult> CreateWithStockErrorAsync(long id, DepotCreateInput depot,
        CancellationToken cancellationToken = default)
    {
        var key = id;

        try
        {
            using var gcts = new GrainCancellationTokenSource();
            using var registration = gcts.RegisterTo(cancellationToken);

            var depotGrain = _clusterClient.GetGrain<IDepotGrain>(key);

            // Depot not save KeyEntity & StateEntity
            // Stock not save KeyEntity & StateEntity
            await depotGrain.CreateWithStockErrorAsync(depot, gcts.Token);
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

    [HttpPost("CreateWithTwoStockError/{id}")]
    public async Task<IActionResult> CreateWithTwoStockErrorAsync(long id, DepotCreateInput depot,
        CancellationToken cancellationToken = default)
    {
        var key = id;

        try
        {
            using var gcts = new GrainCancellationTokenSource();
            using var registration = gcts.RegisterTo(cancellationToken);

            var depotGrain = _clusterClient.GetGrain<IDepotGrain>(key);

            // Depot not save KeyEntity & StateEntity
            // Stock not save KeyEntity & StateEntity
            // todo: test two dependency error
            await depotGrain.CreateWithTwoStockErrorAsync(depot, gcts.Token);
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
