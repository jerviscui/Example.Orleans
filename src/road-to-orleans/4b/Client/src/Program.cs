using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Hosting;
using StackExchange.Redis;

namespace Client
{
    internal class Program
    {
        protected Program()
        {
        }

        private static async Task Main(string[] args)
        {
            var factory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = factory.CreateLogger<Program>();

            var redisConfig = ConfigurationOptions.Parse("host.docker.internal:6379,DefaultDatabase=6,allowAdmin=true");

            await Host.CreateDefaultBuilder(args)
                .UseOrleansClient(clientBuilder =>
                {
                    clientBuilder.UseRedisClustering(options => { options.ConfigurationOptions = redisConfig; });
                    clientBuilder.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "dev";
                        options.ServiceId = "road4b";
                    });

                    clientBuilder.UseConnectionRetryFilter(async (exception, token) =>
                    {
                        logger.LogError(exception, "Connection Retry");
                        try
                        {
                            await Task.Delay(5_000, token);
                        }
                        catch (TaskCanceledException)
                        {
                            // cancell ignored
                        }

                        return true;
                    });
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<HelloWorldClientHostedService>();

                    services.Configure<ConsoleLifetimeOptions>(options => { options.SuppressStatusMessages = true; });
                })
                .ConfigureLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Information);
                    builder.AddConsole();
                })
                .RunConsoleAsync();
        }
    }
}
