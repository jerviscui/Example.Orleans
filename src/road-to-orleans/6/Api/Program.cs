using Api;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Orleans.Configuration;
using StackExchange.Redis;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

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

    private static bool IsDevelopment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?.Equals(Environments.Development, StringComparison.OrdinalIgnoreCase) ?? false;
    }

    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        _ = builder.Services.AddControllers();
        _ = builder.Services.AddEndpointsApiExplorer();
        _ = builder.Services.AddSwaggerGen();

        var domain = "host.docker.internal";
        var redisConfig = ConfigurationOptions.Parse($"{domain}:6379,DefaultDatabase=6,allowAdmin=true");
        if (IsDevelopment())
        {
            domain = "localhost";
            redisConfig = ConfigurationOptions.Parse($"{domain}:6379,DefaultDatabase=7,allowAdmin=true");
        }

        var factory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = factory.CreateLogger<Program>();

        var instance = Environment.GetEnvironmentVariable("HOSTNAME") ?? GetLocalIpAddress().ToString();

        var clusterId = "dev6";
        var serviceId = "road6";

        _ = builder.UseOrleansClient(clientBuilder =>
        {
            _ = clientBuilder.UseRedisClustering(options =>
            {
                options.ConfigurationOptions = redisConfig;
            });
            _ = clientBuilder.Configure<ClusterOptions>(options =>
            {
                options.ClusterId = clusterId;
                options.ServiceId = serviceId;
            });

            _ = clientBuilder.UseConnectionRetryFilter(async (exception, token) =>
            {
                logger.ConnectionFailed();

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

            _ = clientBuilder.AddActivityPropagation();
        });

        _ = builder.Services
            .AddOpenTelemetry()
            .WithMetrics((builder) =>
            {
                _ = builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(
                        "road6api",
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
            })
            .WithTracing(providerBuilder =>
            {
                _ = providerBuilder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(
                        "road6api",
                        serviceVersion: "1.0.0",
                        serviceInstanceId: instance,
                        serviceNamespace: clusterId));

                _ = providerBuilder.SetSampler(new AlwaysOnSampler());

                _ = providerBuilder.AddAspNetCoreInstrumentation();
                // orleans
                _ = providerBuilder.AddSource("Microsoft.Orleans.Runtime");
                _ = providerBuilder.AddSource("Microsoft.Orleans.Application");

                // grpc
                _ = providerBuilder.AddOtlpExporter(options =>
                {
                    options.Protocol = OtlpExportProtocol.Grpc;
                    options.Endpoint = new Uri($"http://{domain}:4317");
                });
            });

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        if (!app.Environment.IsDevelopment())
        {
            // app.UseHsts();
        }

        _ = app.UseSwagger();
        _ = app.UseSwaggerUI();

        // app.UseHttpsRedirection();

        _ = app.UseAuthorization();

        _ = app.MapControllers();

        try
        {
            await app.RunAsync();
        }
        catch (Exception)
        {
            logger.RunError();
        }
    }

    #endregion

    protected Program()
    {
    }
}
