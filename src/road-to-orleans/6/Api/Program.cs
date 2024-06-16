using Api;
using Orleans.Configuration;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#if DEBUG
var redisConfig = ConfigurationOptions.Parse("localhost:6379,DefaultDatabase=7,allowAdmin=true");
#else
var redisConfig = ConfigurationOptions.Parse("host.docker.internal:6379,DefaultDatabase=6,allowAdmin=true");
#endif

var factory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = factory.CreateLogger<Program>();

builder.UseOrleansClient(clientBuilder =>
{
    _ = clientBuilder.UseRedisClustering(options =>
    {
        options.ConfigurationOptions = redisConfig;
    });
    _ = clientBuilder.Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "dev6";
        options.ServiceId = "road6";
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

app.UseAuthorization();

app.MapControllers();

try
{
    await app.RunAsync();
}
catch (Exception)
{
    logger.RunError();
}
