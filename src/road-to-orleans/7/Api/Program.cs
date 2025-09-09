using Interfaces;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Orleans.Configuration;
using Orleans.Serialization;
using Scalar.AspNetCore;
using StackExchange.Redis;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Api;

internal sealed class Program
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
        ?.Equals(Environments.Development, StringComparison.OrdinalIgnoreCase)
               ?? false;
    }

    private static async Task Main(string[] args)
    {
        _ = ThreadPool.SetMinThreads(100, 100);
        _ = ThreadPool.SetMaxThreads(200, 200);

        var builder = WebApplication.CreateBuilder(args);

        _ = builder.Services.AddControllers();
        _ = builder.Services.AddOpenApi();

        _ = builder.Logging.AddJsonConsole();

        var domain = "host.docker.internal";
        var redisConfig = ConfigurationOptions.Parse($"{domain}:6379,DefaultDatabase=6,allowAdmin=true");
        if (IsDevelopment())
        {
            domain = "localhost";
            redisConfig = ConfigurationOptions.Parse($"{domain}:6379,DefaultDatabase=7,allowAdmin=true");
        }

        using var factory = LoggerFactory.Create(builder => builder.AddJsonConsole());
        var logger = factory.CreateLogger<Program>();

        var instance = Environment.GetEnvironmentVariable("HOSTNAME") ?? GetLocalIpAddress().ToString();

        var clusterId = "dev7";
        var serviceId = "road7";

        _ = builder.UseOrleansClient(
            clientBuilder =>
            {
                _ = clientBuilder.UseRedisClustering(
                    options =>
                    {
                        options.ConfigurationOptions = redisConfig;
                    });
                _ = clientBuilder.Configure<ClusterOptions>(
                    options =>
                    {
                        options.ClusterId = clusterId;
                        options.ServiceId = serviceId;
                    });

                _ = clientBuilder.UseConnectionRetryFilter(
                    async (exception, token) =>
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

                _ = clientBuilder.Services
                    .AddSerializer(
                        (serializerBuilder) =>
                        {
                            _ = serializerBuilder.AddJsonSerializer((type) => type == typeof(OrderUpdateInput));
                        })
                    .AddSerializer(
                        (serializerBuilder) =>
                        {
                            _ = serializerBuilder.AddMessagePackSerializer((type) => type == typeof(OrderDeleteInput));
                        })
                    .AddSerializer(
                        (serializerBuilder) =>
                        {
                            _ = serializerBuilder.AddMemoryPackSerializer();
                        });
            });

        _ = builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(
                (builder) => builder.AddService("road7api", clusterId, "1.0.0", serviceInstanceId: instance))
            .WithMetrics(
                (builder) =>
                {
                    _ = builder.AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddProcessInstrumentation();

                    // _ = builder.SetExemplarFilter(ExemplarFilterType.TraceBased);

                    _ = builder.AddOtlpExporter(
                        (exporterOptions, metricReaderOptions) =>
                        {
                            exporterOptions.Endpoint = new Uri($"http://{domain}:9090/api/v1/otlp/v1/metrics");
                            exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                            metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 5_000; // default 60s
                            // metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportTimeoutMilliseconds = 30_000;// default 30s
                        });
                })
            .WithTracing(
                providerBuilder =>
                {
                    _ = providerBuilder.SetSampler(new AlwaysOnSampler());

                    _ = providerBuilder.AddAspNetCoreInstrumentation();
                    // orleans
                    _ = providerBuilder.AddSource("Microsoft.Orleans.Runtime")
                        .AddSource("Microsoft.Orleans.Application");

                    // grpc
                    _ = providerBuilder.AddOtlpExporter(
                        options =>
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

        _ = app.MapOpenApi();
        _ = app.MapScalarApiReference();

        // app.UseHttpsRedirection();

        _ = app.UseAuthorization();

        _ = app.MapControllers();

        try
        {
            await app.RunAsync();
        }
        catch (Exception e)
        {
            logger.RunError(e.Message);
        }
    }

    #endregion

    private Program()
    {
    }
}
