using Interfaces;
using Microsoft.Extensions.Hosting;
using Orleans;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Client;

public class HelloWorldClientHostedService : IHostedService
{
    private readonly IClusterClient _clusterClient;

    public HelloWorldClientHostedService(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    #region IHostedService implementations

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Run HostedService.");

        _ = Task.Run(
            async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        using var cts = new CancellationTokenSource(3_000);

                        // fixme: add to template
                        using var gcts = new GrainCancellationTokenSource();
                        using var reg = gcts.RegisterTo(
                            cts.Token,
                            (t) =>
                            {
                                if (t.IsFaulted)
                                {
                                    Console.WriteLine($"gcts Cancel error: {t.Exception.Message}");
                                }
                            });

                        // var helloWorldGrain = _clusterClient.GetGrain<IHelloWorld>(Random.Shared.Next(30, 40));
                        var helloWorldGrain = _clusterClient.GetGrain<IHelloWorld>(0);
                        // fixme: test GrainCancellationTokenSource
                        Console.WriteLine($"{await helloWorldGrain.SayHelloAsync("Piotr", gcts.Token)}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"SayHelloAsync error: {ex.Message}");
                    }

                    await Task.Delay(10_000, cancellationToken);
                }
            },
            cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    #endregion

}
