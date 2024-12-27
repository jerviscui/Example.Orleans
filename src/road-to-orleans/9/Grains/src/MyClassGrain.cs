using Interfaces;
using Orleans;
using System.Threading.Tasks;

namespace Grains;

public class MyClassGrain : Grain, IMyClassGrain
{
    public MyClassGrain()
    {
    }

    #region IMyClassGrain implementations

    public async Task Method2Async(GrainCancellationToken? token = null)
    {
        var extension = GrainContext.GetGrainExtension<IWatchGrainExtension>();
        await extension.WatchAsync(token.GetCancellationToken());
    }

    #endregion

}
