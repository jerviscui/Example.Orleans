using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Hosting;

namespace Client
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var factory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = factory.CreateLogger<Program>();

            var advertisedIp = Environment.GetEnvironmentVariable("ADVERTISEDIP");
            var siloAdvertisedIpAddress = advertisedIp == null ? GetLocalIpAddress() : IPAddress.Parse(advertisedIp);

            var gatewayPort = Environment.GetEnvironmentVariable("GATEWAYPORT") ?? "30000";
            var arr = gatewayPort.Split(",", StringSplitOptions.RemoveEmptyEntries);
            var endPoints = arr.Select(o => new IPEndPoint(siloAdvertisedIpAddress, int.Parse(o))).ToArray();

            Console.WriteLine(siloAdvertisedIpAddress);
            Console.WriteLine(endPoints);

            await Host.CreateDefaultBuilder(args)
                .UseOrleansClient(clientBuilder =>
                {
                    clientBuilder.UseStaticClustering(endPoints);
                    clientBuilder.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "road4";
                        options.ServiceId = "client";
                    });

                    clientBuilder.UseConnectionRetryFilter(async (exception, token) =>
                    {
                        logger.LogError(exception, "Connection Retry");
                        try
                        {
                            await Task.Delay(5_000, token);
                        }
                        catch (TaskCanceledException)
                        {
                            // cancell ignored
                        }

                        return true;
                    });
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<HelloWorldClientHostedService>();

                    services.Configure<ConsoleLifetimeOptions>(options => { options.SuppressStatusMessages = true; });
                })
                .ConfigureLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Information);
                    builder.AddConsole();
                })
                .RunConsoleAsync();
        }

        private static IPAddress GetLocalIpAddress()
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var network in networkInterfaces)
            {
                if (network.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }

                var properties = network.GetIPProperties();
                if (properties.GatewayAddresses.Count == 0)
                {
                    continue;
                }

                return properties.UnicastAddresses.Where(o =>
                        o.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(o.Address))
                    .Select(o => o.Address)
                    .First();
            }

            throw new NotImplementedException();
        }
    }
}
