using Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using StackExchange.Redis;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SiloHost2;

internal static class Program
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

    public static async Task Main()
    {
        var advertisedIp = Environment.GetEnvironmentVariable("ADVERTISEDIP");
        var advertisedIpAddress = advertisedIp == null ? GetLocalIpAddress() : IPAddress.Parse(advertisedIp);

        var extractedSiloPort = Environment.GetEnvironmentVariable("SILOPORT") ?? "21111";
        var siloPort = int.Parse(extractedSiloPort, CultureInfo.CurrentCulture);

        var extractedGatewayPort = Environment.GetEnvironmentVariable("GATEWAYPORT") ?? "40000";
        var gatewayPort = int.Parse(extractedGatewayPort, CultureInfo.CurrentCulture);

        var instance = Environment.GetEnvironmentVariable(variable: "HOSTNAME") ?? GetLocalIpAddress().ToString();
        instance += $":{extractedSiloPort}";

        var clusterId = "dev6";
        var serviceId = "road6";

#if DEBUG
        var domain = "localhost";
        var redisConfig = ConfigurationOptions.Parse($"{domain}:6379,DefaultDatabase=7,allowAdmin=true");
#else
        var domain = "host.docker.internal";
        var redisConfig = ConfigurationOptions.Parse($"{domain}:6379,DefaultDatabase=6,allowAdmin=true");
#endif

        var host = new HostBuilder()
            .UseOrleans(siloBuilder =>
            {
                _ = siloBuilder.UseDashboard(dashboardOptions =>
                {
                    dashboardOptions.CounterUpdateIntervalMs = 10_000;
                    dashboardOptions.Port = 28080;
                });

                _ = siloBuilder.UseRedisClustering(options => options.ConfigurationOptions = redisConfig);
                _ = siloBuilder.Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = clusterId;
                    options.ServiceId = serviceId;
                });
                _ = siloBuilder.Configure<EndpointOptions>(endpointOptions =>
                {
                    endpointOptions.AdvertisedIPAddress = advertisedIpAddress;
                    endpointOptions.SiloPort = siloPort;
                    endpointOptions.GatewayPort = gatewayPort;
                    endpointOptions.SiloListeningEndpoint = new IPEndPoint(IPAddress.Any, siloPort);
                    endpointOptions.GatewayListeningEndpoint = new IPEndPoint(IPAddress.Any, gatewayPort);
                });

                _ = siloBuilder.UseRedisGrainDirectoryAsDefault(options => options.ConfigurationOptions = redisConfig);
            })

            .ConfigureServices(services => services.AddOpenTelemetry()
                .WithMetrics(builder =>
                {
                    _ = builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService(
                            serviceId,
                            serviceVersion: "1.0.0",
                            serviceInstanceId: instance,
                            serviceNamespace: clusterId));

                    _ = builder.AddMeter("Microsoft.Orleans");

                    _ = builder.AddOtlpExporter((exporterOptions, metricReaderOptions) =>
                    {
                        exporterOptions.Endpoint = new Uri($"http://{domain}:9090/api/v1/otlp/v1/metrics");
                        exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                        metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 5_000; // default 60s
                        // metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportTimeoutMilliseconds = 30_000;// default 30s
                    });
                }))
            .ConfigureLogging(logging => logging.AddConsole())
            .UseConsoleLifetime()
            .Build();

        await host.StartAsync(CancellationToken.None);

        var factory = host.Services.GetRequiredService<IGrainFactory>();
        var grain = factory.GetGrain<IInterGrain>(0);

        await grain.SayInternalAsync("Server2")
            .ContinueWith((t) =>
            {
                Console.WriteLine("SiloHost2 start run:");

                if (t.IsCompletedSuccessfully)
                {
                    Console.WriteLine(t.Result);
                }
                else
                {
                    Console.WriteLine(t.Exception?.Message);
                }
            });

        await host.WaitForShutdownAsync(CancellationToken.None);
    }

    #endregion

}
