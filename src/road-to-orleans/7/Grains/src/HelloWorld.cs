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

    public async Task<string> SayHelloAsync(string name, GrainCancellationToken? token = null)
    {
        token?.CancellationToken.ThrowIfCancellationRequested();
        var interGrain = _grainFactory.GetGrain<IInterGrain>(this.GetPrimaryKeyLong());

        string result;
        try
        {
            result = await interGrain.SayInternalAsync(name, token);
            // or
            // var result = await _client.GetGrain<IInterGrain>(this.GetPrimaryKeyLong()).SayInternalAsync(name, token);
        }
        catch (OperationCanceledException ex)
        {
            Console.WriteLine($"SayInternalAsync Canceled: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SayInternalAsync error: {ex.Message}");
            throw;
        }

        return $"Hello {name}!\n{result}";
    }

    #endregion

}
