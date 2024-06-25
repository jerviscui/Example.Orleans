using Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class RandomKeyController : ControllerBase
{
    private readonly IClusterClient _clusterClient;
    private readonly ILogger<RandomKeyController> _logger;

    public RandomKeyController(ILogger<RandomKeyController> logger, IClusterClient clusterClient)
    {
        _logger = logger;
        _clusterClient = clusterClient;
    }

    #region Methods

    [HttpGet("NoCancel")]
#pragma warning disable CRR0035 // No CancellationToken parameter in the asynchronous method
    public async Task<IActionResult> NoCancelAsync()
#pragma warning restore CRR0035 // No CancellationToken parameter in the asynchronous method
    {
        var key = Random.Shared.Next(1, int.MaxValue);

        try
        {
            var helloWorldGrain = _clusterClient.GetGrain<IHelloWorld>(key);

            Console.WriteLine($"{await helloWorldGrain.SayHelloAsync("Piotr")}");
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "SayHelloAsync Canceled: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SayHelloAsync error: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }

        return Ok($"Hello World! {key}");
    }

    [HttpGet("Say")]
    public Task<IActionResult> SayAsync(CancellationToken cancellationToken = default)
    {
        return SayMaxAsync(int.MaxValue, cancellationToken);
    }

    [HttpGet("SayMax")]
    public async Task<IActionResult> SayMaxAsync([Required][Range(2, int.MaxValue)] int max,
        CancellationToken cancellationToken = default)
    {
        var key = Random.Shared.Next(1, max);

        try
        {
            using var gcts = new GrainCancellationTokenSource();
            using var registration = gcts.RegisterTo(cancellationToken);

            var helloWorldGrain = _clusterClient.GetGrain<IHelloWorld>(key);

            Console.WriteLine($"{await helloWorldGrain.SayHelloAsync("Piotr", gcts.Token)}");
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "SayHelloAsync Canceled: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SayHelloAsync error: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }

        return Ok($"Hello World! {key}");
    }

    #endregion

}
