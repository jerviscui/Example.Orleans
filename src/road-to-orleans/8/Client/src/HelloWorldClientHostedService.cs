using Interfaces;
using Microsoft.Extensions.Hosting;
using Orleans;
using System;
using System.Diagnostics;
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
                    var key = Random.Shared.Next(30, 40);
                    var stopwatch = Stopwatch.StartNew();

                    try
                    {
                        using var cts = new CancellationTokenSource(500);
                        Console.WriteLine($"1: {stopwatch.ElapsedMilliseconds}");

                        using var gcts = new GrainCancellationTokenSource();
                        using var registration = gcts.RegisterTo(
                            cts.Token,
                            (t) =>
                            {
                                if (t.IsFaulted)
                                {
                                    Console.WriteLine($"gcts Cancel error: {t.Exception.Message}");
                                }
                            });

                        Console.WriteLine($"2: {stopwatch.ElapsedMilliseconds}");
                        Console.WriteLine($"2: {DateTime.Now:HH:mm:ss.fff}");

                        var helloWorldGrain = _clusterClient.GetGrain<IHelloWorld>(key);

                        Console.WriteLine($"{await helloWorldGrain.SayHelloAsync("Piotr", gcts.Token)}");
                    }
                    catch (OperationCanceledException ex)
                    {
                        Console.WriteLine($"SayHelloAsync Canceled: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"SayHelloAsync error: {ex.Message}");
                    }

                    Console.WriteLine($"3: {stopwatch.ElapsedMilliseconds}");

                    await Task.Delay(3_000, cancellationToken);
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
