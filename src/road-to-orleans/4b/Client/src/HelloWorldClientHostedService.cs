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
                var helloWorldGrain = _clusterClient.GetGrain<IHelloWorld>(Random.Shared.Next(1, 20));

                // fixme: add to template
                var cts = new GrainCancellationTokenSource();
                _ = cancellationToken.Register(() => cts.Cancel()
                    .ContinueWith((t) =>
                    {
                        if (t.IsFaulted)
                        {
                            Console.WriteLine(t.Exception.Message);//bug: no have cs8602, 测试命中效果
                        }
                    }));

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        // fixme: test GrainCancellationTokenSource
                        Console.WriteLine($"{await helloWorldGrain.SayHelloAsync("Piotr", cts.Token)}");
                    }
                    catch
                    {
                        // ignore
                    }

                    await Task.Delay(1_000, cancellationToken);
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
