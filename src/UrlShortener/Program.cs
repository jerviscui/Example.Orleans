using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Orleans.Configuration;
using StackExchange.Redis;
using UrlShortener;

var builder = WebApplication.CreateBuilder(args);

var redisConfig = ConfigurationOptions.Parse("127.0.0.1:6379,DefaultDatabase=6,allowAdmin=true");

builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.AddRedisGrainStorage(UrlShortenerGrain.Storage,
        options => { options.ConfigurationOptions = redisConfig; });

    //siloBuilder.UseDashboard(options =>
    //{
    //    options.CounterUpdateIntervalMs = 5_000;
    //    options.HistoryLength = 200;
    //});

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

builder.Services.AddOpenTelemetry().WithTracing(providerBuilder =>
{
    providerBuilder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("UrlShortener", "1.0.0"));

    providerBuilder.AddAspNetCoreInstrumentation();
    providerBuilder.AddSource("Microsoft.Orleans");

    providerBuilder.AddZipkinExporter(options => { options.Endpoint = new Uri("http://localhost:9411/api/v2/spans"); });
});

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
