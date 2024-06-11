using Orleans;
using System.Threading.Tasks;

namespace Interfaces;

[Alias("Interfaces.IInterGrain")]
public interface IInterGrain : IGrainWithIntegerKey
{

    #region Methods

    [Alias("SayInternal")]
    public Task<string> SayInternalAsync(string name, GrainCancellationToken? cancellationToken = null);

    #endregion

}
