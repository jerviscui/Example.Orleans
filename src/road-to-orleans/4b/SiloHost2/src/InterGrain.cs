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
            // await Task.Delay(5_000, token.GetCancellationToken());
            await Task.Delay(5_000);
            token?.CancellationToken.ThrowIfCancellationRequested();
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
