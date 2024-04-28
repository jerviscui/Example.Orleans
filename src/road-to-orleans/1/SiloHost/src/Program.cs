using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace SiloHost
{
    internal static class Program
    {
        public static Task Main()
        {
            var siloEndpointConfiguration = GetSiloEndpointConfiguration();

            return new HostBuilder()
                .UseOrleans(siloBuilder =>
                {
                    //Microsoft.Orleans.OrleansTelemetryConsumers.Linux not work
                    //siloBuilder.UseLinuxEnvironmentStatistics();

                    siloBuilder.UseDashboard(dashboardOptions =>
                    {
                        //dashboardOptions.Username = "piotr";
                        //dashboardOptions.Password = "orleans";
                        dashboardOptions.CounterUpdateIntervalMs = 10_000;
                    });

                    siloBuilder.UseLocalhostClustering();
                    siloBuilder.Configure<EndpointOptions>(endpointOptions =>
                    {
                        endpointOptions.AdvertisedIPAddress = siloEndpointConfiguration.Ip;
                        endpointOptions.SiloPort = siloEndpointConfiguration.SiloPort;
                        endpointOptions.GatewayPort = siloEndpointConfiguration.GatewayPort;
                        endpointOptions.SiloListeningEndpoint = new IPEndPoint(IPAddress.Any, 2000);
                        endpointOptions.GatewayListeningEndpoint = new IPEndPoint(IPAddress.Any, 3000);
                    });
                })
                .UseConsoleLifetime()
                .ConfigureLogging(logging => logging.AddConsole())
                .RunConsoleAsync();
        }

        private static SiloEndpointConfiguration GetSiloEndpointConfiguration()
        {
            return new(GetLocalIpAddress(), 2000, 3000);
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