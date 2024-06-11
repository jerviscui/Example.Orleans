using Common;
using Interfaces;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains;

public class HelloWorld : Grain, IHelloWorld
{
#pragma warning disable IDE0052 // Remove unread private members
    private readonly IClusterClient _client;
#pragma warning restore IDE0052 // Remove unread private members
    private readonly IGrainFactory _grainFactory;

    public HelloWorld(IGrainFactory grainFactory, IClusterClient client)
    {
        _grainFactory = grainFactory;
        _client = client;
    }

    #region IHelloWorld implementations

    public async Task<string> SayHelloAsync(string name, GrainCancellationToken? cancellationToken = null)
    {
        await Task.Delay(Random.Shared.Next(10, 50), cancellationToken.GetCancellationToken());

        var result = await _grainFactory.GetGrain<IInterGrain>(this.GetPrimaryKeyLong())
            .SayInternalAsync(name, cancellationToken);
        // or
        // var result = await _client.GetGrain<IInterGrain>(this.GetPrimaryKeyLong()).SayInternalAsync(name, cancellationToken);

        return $"Hello {name}!\n{result}";
    }

    #endregion

}
