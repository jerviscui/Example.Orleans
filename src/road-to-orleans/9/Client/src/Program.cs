using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyNamespace;
using Orleans.Configuration;
using Orleans.Hosting;
using StackExchange.Redis;
using System.Threading;
using System.Threading.Tasks;

namespace Client;

public class Program
{

    #region Constants & Statics

    private static async Task Main(string[] args)
    {
        var factory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = factory.CreateLogger<Program>();

#if DEBUG
        var redisConfig = ConfigurationOptions.Parse("localhost:6379,DefaultDatabase=7,allowAdmin=true");
#else
        var redisConfig = ConfigurationOptions.Parse("host.docker.internal:6379,DefaultDatabase=6,allowAdmin=true");
#endif

        await Host.CreateDefaultBuilder(args)
            .UseOrleansClient(
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
                            options.ClusterId = "dev";
                            options.ServiceId = "road4b";
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
                })
            .ConfigureServices(
                services =>
                {
                    _ = services.AddHostedService<HelloWorldClientHostedService>();

                    _ = services.Configure<ConsoleLifetimeOptions>(
                        options =>
                        {
                            options.SuppressStatusMessages = true;
                        });
                })
            .ConfigureLogging(
                builder =>
                {
                    _ = builder.SetMinimumLevel(LogLevel.Information);
                    _ = builder.AddConsole();
                })
            .RunConsoleAsync(CancellationToken.None);
    }

    #endregion

    protected Program()
    {
    }
}
