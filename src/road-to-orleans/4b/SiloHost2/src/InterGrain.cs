using Interfaces;
using Orleans;
using System.Threading;
using System.Threading.Tasks;

namespace SiloHost2;

public class InterGrain : Grain, IInterGrain
{

    #region IInterGrain implementations

    /// <inheritdoc />
    public async Task<string> SayInternalAsync(string name, GrainCancellationToken? cancellationToken = null)
    {
        await Task.Delay(5, cancellationToken?.CancellationToken ?? CancellationToken.None);

        return $"Internal {name}!";
    }

    #endregion

}
