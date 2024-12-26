using Orleans;
using System.Threading.Tasks;

namespace Interfaces;

[Alias("Interfaces.IMyClassGrain")]
public interface IMyClassGrain : IGrainWithIntegerKey
{

    #region Methods

    [Alias("Method1Async")]
    Task Method1Async(GrainCancellationToken? token = null);

    [Alias("Method2Async")]
    Task Method2Async(GrainCancellationToken? token = null);

    #endregion

}
