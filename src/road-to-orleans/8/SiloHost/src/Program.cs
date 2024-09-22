using Azure.Data.Tables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
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

namespace SiloHost;

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

    public static bool IsDevelopment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?.Equals(Environments.Development, StringComparison.OrdinalIgnoreCase) ?? false;
    }

    public static async Task Main()
    {
        _ = ThreadPool.SetMinThreads(100, 100);
        _ = ThreadPool.SetMaxThreads(200, 200);

        var advertisedIp = Environment.GetEnvironmentVariable("ADVERTISEDIP");
        var advertisedIpAddress = advertisedIp == null ? GetLocalIpAddress() : IPAddress.Parse(advertisedIp);

        var extractedSiloPort = Environment.GetEnvironmentVariable("SILOPORT") ?? "11111";
        var siloPort = int.Parse(extractedSiloPort, CultureInfo.CurrentCulture);

        var extractedGatewayPort = Environment.GetEnvironmentVariable("GATEWAYPORT") ?? "30000";
        var gatewayPort = int.Parse(extractedGatewayPort, CultureInfo.CurrentCulture);

        var instance = Environment.GetEnvironmentVariable("HOSTNAME") ?? GetLocalIpAddress().ToString();
        instance += $":{extractedSiloPort}";

        var clusterId = "dev8";
        var serviceId = "road8";

        var domain = "host.docker.internal";
        var redisConfig = ConfigurationOptions.Parse($"{domain}:6379,DefaultDatabase=6,allowAdmin=true");
        if (IsDevelopment())
        {
            domain = "localhost";
            redisConfig = ConfigurationOptions.Parse($"{domain}:6379,DefaultDatabase=7,allowAdmin=true");
        }

        var host = new HostBuilder()
            .UseOrleans(siloBuilder =>
            {
                _ = siloBuilder.UseDashboard(dashboardOptions => dashboardOptions.CounterUpdateIntervalMs = 10_000);

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

                _ = siloBuilder.AddActivityPropagation();

                _ = siloBuilder.AddAdoNetGrainStorageAsDefault((storageOptions) =>
                {
                    storageOptions.Invariant = "Npgsql"; // Orleans.Persistence.AdoNet.Storage.AdoNetInvariants.InvariantNamePostgreSql
                    storageOptions.ConnectionString = $"Host={domain};Port=5432;Database=orleans;Username=postgres;Password=123456;";
                });

                _ = siloBuilder.UseTransactions();
                _ = siloBuilder.AddAzureTableTransactionalStateStorage(
                    "AzureTable",
                    (options) =>
                    {
                        options.TableServiceClient = new TableServiceClient(string.Empty);
                    });
            })
            .ConfigureServices(services =>
                services.AddOpenTelemetry()
                .ConfigureResource((builder) =>
                    builder.AddService("road8", clusterId, "1.0.0", serviceInstanceId: instance))
                .WithMetrics(builder =>
                {
                    _ = builder.AddMeter("Microsoft.Orleans");

                    _ = builder.AddOtlpExporter((exporterOptions, metricReaderOptions) =>
                    {
                        exporterOptions.Endpoint = new Uri($"http://{domain}:9090/api/v1/otlp/v1/metrics");
                        exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                        metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 5_000; // default 60s
                        // metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportTimeoutMilliseconds = 30_000;// default 30s
                    });
                })
                .WithTracing(providerBuilder =>
                {
                    _ = providerBuilder.SetSampler(new AlwaysOnSampler());

                    // orleans
                    _ = providerBuilder.AddSource("Microsoft.Orleans.Runtime")
                        .AddSource("Microsoft.Orleans.Application");

                    // grpc
                    _ = providerBuilder.AddOtlpExporter(options =>
                    {
                        options.Protocol = OtlpExportProtocol.Grpc;
                        options.Endpoint = new Uri($"http://{domain}:4317");
                    });
                }))
            .ConfigureLogging(logging => logging.AddConsole())
            .UseConsoleLifetime()
            .Build();

        await host.StartAsync(CancellationToken.None);

        await host.WaitForShutdownAsync(CancellationToken.None);
    }

    #endregion

}
