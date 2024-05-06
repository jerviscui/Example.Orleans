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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SiloHost2
{
    public interface MyInterface
    {

    }

    public class MyClass2
    {
        public required string PropertyName
        {
            get; set;
        }
        public required string PropertyName2
        {
            get;
            set;
        }
        public required object PropertyName3
        {
            get
                ; set;
        }
        public required object PropertyName4
        {
            get; set
                ;
        }
        public required object PropertyName5
        {
            get; set;
        }

        private static Task<string> MethodNameAsync(int value, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var tasks = new List<Task>();
            for (var i = 0; i < 10; i++)
            {
                tasks.Add(Task.Delay(100));
            }

            Task.WaitAll(tasks.ToArray());  // CRR0037

            return Task.FromResult(value.ToString());
        }
    }

    internal static class Program
    {
        private static Task<int> Run() => Task.FromResult(1);
        private static Task<int> Run(int i) => Task.FromResult(i);
        private static Task<int> Run(int i, int y, DateTime dateTime) => Task.FromResult(i);
        public static Task<int> Run2(int i) => Task.FromResult(i);

        #region MyRegion
        private static readonly int X;
        private static readonly int y = 1;
        public static async Task Main()
        {

            var my = new MyClass2() { PropertyName = "", PropertyName2 = string.Empty, PropertyName3 = 3, PropertyName4 = 4, PropertyName5 = 5 };
            var n = await Run(y);


            //run
            _ = await Run();
            var v = await Run();
            _ = GetLocalIpAddress();

            var d = new DateTime(1, DateTimeKind.Local);
            var date = d.Date;
            if (v > 0)
            {
                await Main().ConfigureAwait(false);
            }
abc:

            var arr = new int[] { 1 };
            arr[1] = 1;

            //goto abc;

            var advertisedIp = Environment.GetEnvironmentVariable("ADVERTISEDIP");
            var advertisedIpAddress = advertisedIp == null ? GetLocalIpAddress() : IPAddress.Parse(advertisedIp);

            var extractedSiloPort = Environment.GetEnvironmentVariable("SILOPORT") ?? "21111";
            var siloPort = int.Parse(extractedSiloPort, CultureInfo.CurrentCulture);

            var extractedGatewayPort = Environment.GetEnvironmentVariable("GATEWAYPORT") ?? "40000";
            var gatewayPort = int.Parse(extractedGatewayPort, CultureInfo.CurrentCulture);

            var clusterId = "dev";
            var instance = Environment.GetEnvironmentVariable(variable: "HOSTNAME") ?? GetLocalIpAddress().ToString();
            instance += ":" + extractedSiloPort;

            var redisConfig = ConfigurationOptions.Parse("host.docker.internal:6379,DefaultDatabase=6,allowAdmin=true");

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
                        options.ServiceId = "road4b";
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

                .ConfigureServices(services =>
                    services.AddOpenTelemetry().WithMetrics(builder =>
                    {

                        _ = builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                            .AddService("road4b2", serviceVersion: "1.0.0",
                                serviceInstanceId: instance, serviceNamespace: clusterId));

                        _ = builder.AddMeter("Microsoft.Orleans");

                        _ = builder.AddOtlpExporter((exporterOptions, metricReaderOptions) =>
                        {
                            exporterOptions.Endpoint =
                                new Uri("http://host.docker.internal:9090/api/v1/otlp/v1/metrics");
                            exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                            metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds =
                                5_000; // default 60s
                            //metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportTimeoutMilliseconds = 30_000;// default 30s
                        });
                    }))
                .ConfigureLogging(logging => logging.AddConsole())
                .UseConsoleLifetime()
                .Build();

            await host.StartAsync();

            var factory = host.Services.GetRequiredService<IGrainFactory>();
            var grain = factory.GetGrain<IInterGrain>(0);
            Console.WriteLine(await grain.SayInternal("Server2"));

            await host.WaitForShutdownAsync();
        }
        #endregion

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

            throw new NotImplementedException();

        }

    }

}


