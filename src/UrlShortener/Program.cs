using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Orleans.Configuration;
using StackExchange.Redis;
using UrlShortener;

var builder = WebApplication.CreateBuilder(args);

var redisConfig = ConfigurationOptions.Parse("127.0.0.1:6379,DefaultDatabase=6,allowAdmin=true");
IConnectionMultiplexer connection = ConnectionMultiplexer.Connect(redisConfig);

Task<IConnectionMultiplexer> GetRedisConnection() => Task.FromResult(connection);

builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.AddRedisGrainStorage(UrlShortenerGrain.Storage,
        options =>
        {
            options.ConfigurationOptions = redisConfig;
            options.CreateMultiplexer = _ => GetRedisConnection();
        });

    // http://localhost:8080/
    siloBuilder.UseDashboard(options =>
    {
        options.CounterUpdateIntervalMs = 5_000;
        options.HistoryLength = 200;
    });

    siloBuilder.UseLocalhostClustering();

    siloBuilder.Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "dev";
        options.ServiceId = "UrlApp";
    });
    siloBuilder.UseRedisClustering(options => { options.ConfigurationOptions = redisConfig; });

    siloBuilder.AddRedisGrainDirectory(UrlGrain.DistributedDirectory,
        options => { options.ConfigurationOptions = redisConfig; });

    siloBuilder.AddActivityPropagation();
});

builder.Services.AddOpenTelemetry()
    .WithTracing(providerBuilder =>
    {
        providerBuilder.SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService("UrlShortener", serviceVersion: "1.0.0", serviceInstanceId: "local"));

        providerBuilder.SetSampler(new AlwaysOnSampler());

        providerBuilder.AddAspNetCoreInstrumentation();
        providerBuilder.AddRedisInstrumentation();
        //orleans
        providerBuilder.AddSource("Microsoft.Orleans.Runtime");
        providerBuilder.AddSource("Microsoft.Orleans.Application");

        //providerBuilder.AddZipkinExporter(options => { options.Endpoint = new Uri("http://localhost:9411/api/v2/spans"); });
        // grpc
        providerBuilder.AddOtlpExporter(options =>
        {
            options.Protocol = OtlpExportProtocol.Grpc;
            options.Endpoint = new Uri("http://localhost:4317");
        });
        // http
        //providerBuilder.AddOtlpExporter(options => { options.Protocol = OtlpExportProtocol.HttpProtobuf; });
    })
    .WithMetrics(providerBuilder =>
    {
        providerBuilder.SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService("UrlShortener", serviceVersion: "1.0.0", serviceInstanceId: "local"));

        providerBuilder.AddMeter("Microsoft.Orleans");

        //providerBuilder.AddPrometheusExporter(options => { options.DisableTotalNameSuffixForCounters = true; });
        providerBuilder.AddOtlpExporter((exporterOptions, metricReaderOptions) =>
        {
            exporterOptions.Endpoint =
                new Uri("http://localhost:9090/api/v1/otlp/v1/metrics");
            exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
            metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 5_000;
        });
    });

builder.Services.AddSingleton(connection);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//app.UseOpenTelemetryPrometheusScrapingEndpoint(); // http://host.docker.internal:5000/metrics
app.Run();
