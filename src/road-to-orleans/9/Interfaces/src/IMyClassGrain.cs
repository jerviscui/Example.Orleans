using Orleans;
using System.Threading.Tasks;

namespace Interfaces;

[Alias("Interfaces.IMyClassGrain")]
public interface IMyClassGrain : IGrainWithIntegerKey
{

    #region Methods

    [Alias("Method2Async")]
    Task Method2Async(GrainCancellationToken? token = null);

    #endregion

}
