using System;
using Grains;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;

[assembly: GenerateCodeForDeclaringAssembly(typeof(YourReminderGrain))]

await new HostBuilder()
    .UseOrleans(builder =>
    {
        builder.UseLocalhostClustering();
        builder.UseInMemoryReminderService();
        builder.UseDashboard();
        builder.ConfigureServices(services =>
            services.AddHostedService<GrainActivatorHostedService.GrainActivatorHostedService>());
    })
    .RunConsoleAsync();

Console.ReadLine();
