using Orleans;
using System.Threading;

namespace SiloHost2;

public static class GrainCancellationTokenExtensions
{

    #region Constants & Statics

    public static CancellationToken MethodName(this GrainCancellationToken cancellationToken)
    {
        return cancellationToken?.CancellationToken ?? CancellationToken.None;
    }

    #endregion

}
