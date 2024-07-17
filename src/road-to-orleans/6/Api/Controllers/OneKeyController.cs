using Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OneKeyController : ControllerBase
{
    private readonly IClusterClient _clusterClient;
    private readonly ILogger<OneKeyController> _logger;

    public OneKeyController(ILogger<OneKeyController> logger, IClusterClient clusterClient)
    {
        _logger = logger;
        _clusterClient = clusterClient;
    }

    #region Methods

    [HttpGet("Say")]
    public Task<IActionResult> Say2Async([Required][Range(1, int.MaxValue)] int key,
        CancellationToken cancellationToken = default)
    {
        return SayAsync(key, cancellationToken);
    }

    [HttpGet("Say/{key}")]
    public async Task<IActionResult> SayAsync([Required][Range(1, int.MaxValue)] int key,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var gcts = new GrainCancellationTokenSource();
            using var registration = gcts.RegisterTo(cancellationToken);

            var helloWorldGrain = _clusterClient.GetGrain<IHelloWorld>(key);

            Console.WriteLine($"{await helloWorldGrain.SayHelloAsync("Piotr", gcts.Token)}");
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

        return Ok($"Hello World! {key}");
    }

    #endregion

}
