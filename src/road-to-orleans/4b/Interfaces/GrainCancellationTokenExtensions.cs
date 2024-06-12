using Orleans;
using System.Threading;

namespace Interfaces;

public static class GrainCancellationTokenExtensions
{

    #region Constants & Statics

    /// <summary>
    /// Gets CancellationToken or None.
    /// </summary>
    public static CancellationToken GetCancellationToken(this GrainCancellationToken? cancellationToken)
    {
        return cancellationToken?.CancellationToken ?? CancellationToken.None;
    }

    #endregion

}
