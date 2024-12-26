using Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MyClassController : ControllerBase
{
    private readonly IClusterClient _clusterClient;
    private readonly ILogger<MyClassController> _logger;

    public MyClassController(ILogger<MyClassController> logger, IClusterClient clusterClient)
    {
        _logger = logger;
        _clusterClient = clusterClient;
    }

    #region Methods

    [HttpPost("Method1")]
    public async Task<IActionResult> Method1Async(CancellationToken cancellationToken = default)
    {
        var key = 0;

        try
        {
            using var gcts = new GrainCancellationTokenSource();
            using var registration = gcts.RegisterTo(cancellationToken);

            var myClassGrain = _clusterClient.GetGrain<IMyClassGrain>(key);

            await myClassGrain.Method1Async(gcts.Token);
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

        return Ok($"Method1Async ok");
    }

    [HttpPost("Method2")]
    public async Task<IActionResult> Method2Async(CancellationToken cancellationToken = default)
    {
        var key = 0;

        try
        {
            using var gcts = new GrainCancellationTokenSource();
            using var registration = gcts.RegisterTo(cancellationToken);

            var myClassGrain = _clusterClient.GetGrain<IMyClassGrain>(key);

            await myClassGrain.Method2Async(gcts.Token);
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

        return Ok($"Method2Async ok");
    }

    #endregion

}
