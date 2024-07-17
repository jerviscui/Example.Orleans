using Interfaces;
using Orleans;
using System;
using System.Threading.Tasks;

namespace SiloHost2;

public class InterGrain : Grain, IInterGrain
{

    #region IInterGrain implementations

    /// <inheritdoc />
    public async Task<string> SayInternalAsync(string name, GrainCancellationToken? token = null)
    {
        try
        {
            await Task.Delay(10, token.GetCancellationToken());
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"Canceled: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }

        return $"Internal {name}!";
    }

    #endregion

}
