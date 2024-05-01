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
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SiloHost;

internal class MyClass
{
    protected MyClass()
    {

    }
}
#region B
internal class MyClassB : MyClass, IDisposable
{
    private void ReleaseUnmanagedResources() => throw new NotSupportedException();

    protected virtual void Dispose(bool disposing) => ReleaseUnmanagedResources();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~MyClassB() => Dispose(false);
}
#endregion

internal class Program
{
    public void Test() => throw new NotSupportedException();
    #region BBB
    private static void MethodName() => throw new NotSupportedException();

    public void MethodName3()
    {
        MethodName();
        myVarb = 1;
        Console.WriteLine(MyProperty);
    }

    #endregion
    private int myVarb;

    public int MyProperty { get; set; }

    public static async Task Main()
    {

        var advertisedIp = Environment.GetEnvironmentVariable("ADVERTISEDIP");
        var advertisedIpAddress = advertisedIp == null ? GetLocalIpAddress() : IPAddress.Parse(advertisedIp);

        var extractedSiloPort = Environment.GetEnvironmentVariable("SILOPORT") ?? "11111";
        var siloPort = int.Parse(extractedSiloPort);

        var extractedGatewayPort = Environment.GetEnvironmentVariable("GATEWAYPORT") ?? "30000";
        var gatewayPort = int.Parse(extractedGatewayPort);

        var instance = Environment.GetEnvironmentVariable("HOSTNAME") ?? GetLocalIpAddress().ToString();
        instance += ":" + extractedSiloPort;

        var clusterId = "dev";
        var redisConfig = ConfigurationOptions.Parse("host.docker.internal:6379,DefaultDatabase=6,allowAdmin=true");

        var host = new HostBuilder()
            .UseOrleans(siloBuilder =>
            {
                siloBuilder.UseDashboard(dashboardOptions =>
                {
                    dashboardOptions.CounterUpdateIntervalMs = 10_000;
                });

                siloBuilder.UseRedisClustering(options => { options.ConfigurationOptions = redisConfig; });
                siloBuilder.Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = clusterId;
                    options.ServiceId = "road4b";
                });
                siloBuilder.Configure<EndpointOptions>(endpointOptions =>
                {
                    endpointOptions.AdvertisedIPAddress = advertisedIpAddress;
                    endpointOptions.SiloPort = siloPort;
                    endpointOptions.GatewayPort = gatewayPort;
                    endpointOptions.SiloListeningEndpoint = new IPEndPoint(IPAddress.Any, siloPort);
                    endpointOptions.GatewayListeningEndpoint = new IPEndPoint(IPAddress.Any, gatewayPort);
                });

                siloBuilder.UseRedisGrainDirectoryAsDefault(options => options.ConfigurationOptions = redisConfig);
            })
                .ConfigureServices(services =>
                {
                    services.AddOpenTelemetry().WithMetrics(builder =>
                    {
                        builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService("road4b", serviceVersion: "1.0.0",
                            serviceInstanceId: instance, serviceNamespace: clusterId));

                        builder.AddMeter("Microsoft.Orleans");

                        builder.AddOtlpExporter((exporterOptions, metricReaderOptions) =>
                        {
                            exporterOptions.Endpoint =
                                new Uri("http://host.docker.internal:9090/api/v1/otlp/v1/metrics");
                            exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                            metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds =
                                5_000; // default 60s
                            //metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportTimeoutMilliseconds = 30_000;// default 30s
                        });
                    });
                })
            .ConfigureLogging(logging => logging.AddConsole())
            .UseConsoleLifetime()
            .Build();

        await host.StartAsync();

        //var factory = host.Services.GetRequiredService<IGrainFactory>();
        //var grain = factory.GetGrain<IHelloWorld>(0);
        //Console.WriteLine(await grain.SayHello("Server"));

        await host.WaitForShutdownAsync();
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
