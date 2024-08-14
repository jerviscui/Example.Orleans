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

    public HelloWorld(IClusterClient client)
    {
        _client = client;
    }

    #region IHelloWorld implementations

    public async Task<string> SayHelloAsync(string name, GrainCancellationToken? token = null)
    {
        Console.WriteLine($"2: {DateTime.Now:HH:mm:ss.fff}");

        token?.CancellationToken.ThrowIfCancellationRequested();
        var interGrain = GrainFactory.GetGrain<IInterGrain>(this.GetPrimaryKeyLong());

        string result;
        try
        {
            result = await interGrain.SayInternalAsync(name, token);
            // or
            // var result = await _client.GetGrain<IInterGrain>(this.GetPrimaryKeyLong()).SayInternalAsync(name, token);
        }
        catch (OperationCanceledException ex)
        {
            Console.WriteLine($"3: {DateTime.Now:HH:mm:ss.fff}");

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
