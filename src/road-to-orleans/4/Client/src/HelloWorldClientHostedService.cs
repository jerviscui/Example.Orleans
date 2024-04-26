using System;
using System.Threading;
using System.Threading.Tasks;
using Interfaces;
using Microsoft.Extensions.Hosting;
using Orleans;

namespace Client
{
    public class HelloWorldClientHostedService : IHostedService
    {
        private readonly IClusterClient _clusterClient;

        public HelloWorldClientHostedService(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var helloWorldGrain = _clusterClient.GetGrain<IHelloWorld>(Random.Shared.Next(1, 20));
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        Console.WriteLine($"{await helloWorldGrain.SayHello("Piotr")}");
                    }
                    catch
                    {
                        // ignore
                    }

                    await Task.Delay(1_000, cancellationToken);
                }
            }, cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
