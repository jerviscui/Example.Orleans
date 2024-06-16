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
        Console.WriteLine($"2: {DateTime.Now:HH:mm:ss.fff}");

        try
        {
            await Task.Delay(1_000, token.GetCancellationToken());
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"3: {DateTime.Now:HH:mm:ss.fff}");

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
