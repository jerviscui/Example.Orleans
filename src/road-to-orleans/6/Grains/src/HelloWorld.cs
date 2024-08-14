using Interfaces;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains;

public class HelloWorld : Grain, IHelloWorld
{

    #region IHelloWorld implementations

    public async Task<string> SayHelloAsync(string name, GrainCancellationToken? token = null)
    {
        token?.CancellationToken.ThrowIfCancellationRequested();
        var interGrain = GrainFactory.GetGrain<IInterGrain>(this.GetPrimaryKeyLong());

        string result;
        try
        {
            result = await interGrain.SayInternalAsync(name, token);
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
