using Orleans;

namespace Common;

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
