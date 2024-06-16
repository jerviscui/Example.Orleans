using Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloWorldController : ControllerBase
{
    private readonly IClusterClient _clusterClient;
    private readonly ILogger<HelloWorldController> _logger;

    public HelloWorldController(ILogger<HelloWorldController> logger, IClusterClient clusterClient)
    {
        _logger = logger;
        _clusterClient = clusterClient;
    }

    #region Methods

    [HttpGet(Name = "Say")]
    public async Task<IActionResult> SayAsync(CancellationToken cancellationToken = default)
    {
        var key = Random.Shared.Next();

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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SayHelloAsync error: {Message}", ex.Message);
        }

        return Ok($"Hello World! {key}");
    }

    #endregion

}
