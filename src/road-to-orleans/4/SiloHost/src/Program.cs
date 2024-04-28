using System;
using System.Collections.Generic;
using System.Linq;
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
using OpenTelemetry.Resources;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace SiloHost
{
    internal static class Program
    {
        public static async Task Main()
        {
            var advertisedIp = Environment.GetEnvironmentVariable("ADVERTISEDIP");
            var advertisedIpAddress = advertisedIp == null ? GetLocalIpAddress() : IPAddress.Parse(advertisedIp);

            var extractedSiloPort = Environment.GetEnvironmentVariable("SILOPORT") ?? "11111";
            var siloPort = int.Parse(extractedSiloPort);

            var extractedGatewayPort = Environment.GetEnvironmentVariable("GATEWAYPORT") ?? "30000";
            var gatewayPort = int.Parse(extractedGatewayPort);

            // For the sake of simplicity, a primary silo is used here (even though all silos are peers in the cluster) as in-memory cluster membership emulation was utilised in this example.
            // If the primary address is not provided, we're assuming all silos in the cluster are running under one IP.
            var primaryAddress = Environment.GetEnvironmentVariable("PRIMARYADDRESS");
            var primaryIp = primaryAddress == null ? advertisedIpAddress : IPAddress.Parse(primaryAddress);

            var primaryPort = Environment.GetEnvironmentVariable("PRIMARYPORT");
            var primarySiloPort = primaryPort == null ? siloPort : int.Parse(primaryPort);

            Console.WriteLine(advertisedIpAddress);
            Console.WriteLine(gatewayPort);

            var instance = Environment.GetEnvironmentVariable("HOSTNAME") ?? GetLocalIpAddress().ToString();
            instance += ":" + extractedSiloPort;

            var host = new HostBuilder()
                .UseOrleans(siloBuilder =>
                {
                    siloBuilder.UseDashboard(dashboardOptions =>
                    {
                        //dashboardOptions.Username = "piotr";
                        //dashboardOptions.Password = "orleans";
                        dashboardOptions.CounterUpdateIntervalMs = 10_000;
                    });
                    siloBuilder.UseLocalhostClustering(
                        primarySiloEndpoint: new IPEndPoint(primaryIp, primarySiloPort));
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
                        endpointOptions.SiloListeningEndpoint = new IPEndPoint(IPAddress.Any, siloPort);
                        endpointOptions.GatewayListeningEndpoint = new IPEndPoint(IPAddress.Any, gatewayPort);
                    });
                })
                .ConfigureServices(services =>
                {
                    services.AddOpenTelemetry().WithMetrics(builder =>
                    {
                        builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                            .AddService("server", serviceVersion: "1.0.0",
                                serviceInstanceId: instance,
                                serviceNamespace: "dev")
                            .AddAttributes([new KeyValuePair<string, object>("cluster", "road4")]));

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

                return properties.UnicastAddresses.Where(o =>
                        o.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(o.Address))
                    .Select(o => o.Address)
                    .First();
            }

            throw new NotImplementedException();
        }
    }
}
