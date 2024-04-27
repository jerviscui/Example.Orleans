using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Hosting;
using StackExchange.Redis;

namespace Client
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var factory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = factory.CreateLogger<Program>();

            var redisConfig = ConfigurationOptions.Parse("127.0.0.1:6379,DefaultDatabase=6,allowAdmin=true");

            await Host.CreateDefaultBuilder(args)
                .UseOrleansClient(clientBuilder =>
                {
                    clientBuilder.UseRedisClustering(options => { options.ConfigurationOptions = redisConfig; });
                    clientBuilder.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "road4b";
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

                foreach (var address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily == AddressFamily.InterNetwork &&
                        !IPAddress.IsLoopback(address.Address))
                    {
                        return address.Address;
                    }
                }
            }

            return null;
        }
    }
}
