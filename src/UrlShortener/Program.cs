using Microsoft.AspNetCore.Mvc;
using Orleans.Configuration;
using StackExchange.Redis;
using UrlShortener;

var builder = WebApplication.CreateBuilder(args);

var redisConfig = ConfigurationOptions.Parse("10.99.59.47:7000,DefaultDatabase=6,allowAdmin=true");

builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.AddRedisGrainStorage(UrlShortenerGrain.Storage,
        options => { options.ConfigurationOptions = redisConfig; });

    siloBuilder.UseDashboard(options =>
    {
        //options.CounterUpdateIntervalMs = 5_000;
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
});

builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

builder.Services.AddControllers();
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
