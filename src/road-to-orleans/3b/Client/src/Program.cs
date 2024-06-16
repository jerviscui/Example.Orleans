using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Client;

internal class Program
{

    #region Constants & Statics

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

            return properties.UnicastAddresses
                .Where(o => o.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(o.Address))
                .Select(o => o.Address)
                .First();
        }

        throw new NotImplementedException();
    }

    private static async Task Main(string[] args)
    {
        var factory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = factory.CreateLogger<Program>();

        var advertisedIp = Environment.GetEnvironmentVariable("ADVERTISEDIP");
        var siloAdvertisedIpAddress = advertisedIp == null ? GetLocalIpAddress() : IPAddress.Parse(advertisedIp);

        var siloGatewayPort = int.Parse(
            Environment.GetEnvironmentVariable("GATEWAYPORT") ?? "30000",
            CultureInfo.CurrentCulture);

        Console.WriteLine(siloAdvertisedIpAddress);
        Console.WriteLine(siloGatewayPort);

        await Host.CreateDefaultBuilder(args)
            .UseOrleansClient(clientBuilder =>
            {
                _ = clientBuilder.UseStaticClustering(new IPEndPoint(siloAdvertisedIpAddress, siloGatewayPort));
                _ = clientBuilder.Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "road3";
                    options.ServiceId = "client";
                });

                _ = clientBuilder.UseConnectionRetryFilter(async (exception, token) =>
                {
                    logger.LogError(exception, "Connection Retry");
                    try
                    {
                        await Task.Delay(5_000, token);
                    }
                    catch (TaskCanceledException)
                    {
                        return false;
                    }

                    return true;
                });
            })
            .ConfigureServices(services =>
            {
                _ = services.AddHostedService<HelloWorldClientHostedService>();

                _ = services.Configure<ConsoleLifetimeOptions>(options =>
                {
                    options.SuppressStatusMessages = true;
                });
            })
            .ConfigureLogging(builder =>
            {
                _ = builder.SetMinimumLevel(LogLevel.Information);
                _ = builder.AddConsole();
            })
            .RunConsoleAsync();
    }

    #endregion

    protected Program()
    {
    }
}
