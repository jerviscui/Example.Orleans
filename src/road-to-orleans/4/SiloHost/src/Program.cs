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
            var advertisedIpAddress = advertisedIp == null ? GetLocalIpAddress() : IPAddress.Parse(advertisedIp);

            var extractedSiloPort = Environment.GetEnvironmentVariable("SILOPORT") ?? "2000";
            var siloPort = int.Parse(extractedSiloPort);

            var extractedGatewayPort = Environment.GetEnvironmentVariable("GATEWAYPORT") ?? "3000";
            var gatewayPort = int.Parse(extractedGatewayPort);

            // For the sake of simplicity, a primary silo is used here (even though all silos are peers in the cluster) as in-memory cluster membership emulation was utilised in this example.
            // If the primary address is not provided, we're assuming all silos in the cluster are running under one IP.
            var primaryAddress = Environment.GetEnvironmentVariable("PRIMARYADDRESS");
            var primaryIp = primaryAddress == null ? advertisedIpAddress : IPAddress.Parse(primaryAddress);

            var extractedPrimaryPort = Environment.GetEnvironmentVariable("PRIMARYPORT") ??
                throw new Exception("Primary port cannot be null");
            var developmentPeerPort = int.Parse(extractedPrimaryPort);

            var extractDashboardPort = Environment.GetEnvironmentVariable("DASHBOARDPORT") ??
                throw new Exception("Dashboard port cannot be null");
            var dashboardPort = int.Parse(extractDashboardPort);

            var host = new HostBuilder()
                .UseOrleans(siloBuilder =>
                {
                    siloBuilder.UseDashboard(dashboardOptions =>
                    {
                        //dashboardOptions.Username = "piotr";
                        //dashboardOptions.Password = "orleans";
                        dashboardOptions.CounterUpdateIntervalMs = 10_000;
                        dashboardOptions.Port = dashboardPort;
                    });
                    siloBuilder.UseLocalhostClustering(
                        primarySiloEndpoint: new IPEndPoint(primaryIp, developmentPeerPort));
                    siloBuilder.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "road4";
                        options.ServiceId = "server";
                    });
                    siloBuilder.Configure<EndpointOptions>(endpointOptions =>
                    {
                        endpointOptions.AdvertisedIPAddress = advertisedIpAddress;
                        endpointOptions.SiloPort = siloPort;
                        endpointOptions.GatewayPort = gatewayPort;
                        endpointOptions.SiloListeningEndpoint = new IPEndPoint(IPAddress.Any, siloPort + 10_000);
                        endpointOptions.GatewayListeningEndpoint = new IPEndPoint(IPAddress.Any, gatewayPort + 10_000);
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
