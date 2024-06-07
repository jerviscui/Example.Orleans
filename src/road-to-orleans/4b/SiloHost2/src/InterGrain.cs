using Interfaces;
using Orleans;
using System.Threading.Tasks;

namespace SiloHost2;

public class InterGrain : Grain, IInterGrain
{

    #region IInterGrain implementations

    /// <inheritdoc />
    public async Task<string> SayInternal(string name)
    {
        await Task.Delay(5);

        return $"Internal {name}!";
    }

    #endregion

}
