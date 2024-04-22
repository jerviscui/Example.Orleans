using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace SiloHost
{
    internal class Program
    {
        public static async Task Main()
        {
            var advertisedIp = Environment.GetEnvironmentVariable("ADVERTISEDIP");
            var siloEndpointConfiguration = GetSiloEndpointConfiguration();

            var host = new HostBuilder()
                .UseOrleans(siloBuilder =>
                {
                    siloBuilder.UseDashboard(dashboardOptions =>
                    {
                        //dashboardOptions.Username = "piotr";
                        //dashboardOptions.Password = "orleans";
                        dashboardOptions.CounterUpdateIntervalMs = 10_000;
                    });

                    siloBuilder.UseLocalhostClustering();
                    siloBuilder.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "road3";
                        options.ServiceId = "server";
                    });
                    siloBuilder.Configure<EndpointOptions>(endpointOptions =>
                    {
                        endpointOptions.AdvertisedIPAddress = advertisedIp is null
                            ? siloEndpointConfiguration.Ip
                            : IPAddress.Parse(advertisedIp);
                        endpointOptions.SiloPort = siloEndpointConfiguration.SiloPort;
                        endpointOptions.GatewayPort = siloEndpointConfiguration.GatewayPort;
                        endpointOptions.SiloListeningEndpoint =
                            new IPEndPoint(IPAddress.Any, siloEndpointConfiguration.SiloPort);
                        endpointOptions.GatewayListeningEndpoint =
                            new IPEndPoint(IPAddress.Any, siloEndpointConfiguration.GatewayPort);
                    });
                })
                .ConfigureServices(services =>
                {
                    services.AddOpenTelemetry().WithMetrics(builder =>
                    {
                        builder.AddMeter("Microsoft.Orleans");

                        //builder.AddConsoleExporter();
                        builder.AddOtlpExporter((exporterOptions, metricReaderOptions) =>
                        {
                            exporterOptions.Endpoint =
                                new Uri("http://localhost:9090/api/v1/otlp/v1/metrics");
                            exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                            metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 5_000;
                        });
                    });
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .UseConsoleLifetime()
                .Build();

            await host.StartAsync();

            var factory = host.Services.GetRequiredService<IGrainFactory>();
            var grain = factory.GetGrain<IHelloWorld>(0);
            Console.WriteLine(await grain.SayHello("Server"));

            await host.WaitForShutdownAsync();
        }

        private static SiloEndpointConfiguration GetSiloEndpointConfiguration()
        {
            return new SiloEndpointConfiguration(GetLocalIpAddress(), 2000, 3000);
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
