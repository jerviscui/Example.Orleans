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

        _ = Task
            .Run(
                async () =>
                {
                    var helloWorldGrain = _clusterClient.GetGrain<IHelloWorld>(Random.Shared.Next(1, 20));

                    var cts = new GrainCancellationTokenSource();
                    _ = cancellationToken.Register(() => cts.Cancel()
                        .ContinueWith((t) =>
                        {
                            if (t.IsFaulted)
                            {
                                Console.WriteLine(t.Exception.Message);
                            }
                        }));

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            Console.WriteLine($"{await helloWorldGrain.SayHelloAsync("Piotr", cts.Token)}");
                        }
                        catch
                        {
                            // ignore
                        }

                        await Task.Delay(1_000, cancellationToken);

                        // bug: throw will be crash?
                        throw new NotSupportedException("test task crash");
                    }
                },
                cancellationToken);
        // .ContinueWith(
        // (t) =>
        // {
        // if (t.IsFaulted)
        // {
        // Console.WriteLine(t.Exception?.Message);
        // }
        // },
        // cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    #endregion

}
