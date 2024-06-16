using Orleans;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Interfaces;

public static class GrainCancellationTokenExtensions
{

    #region Constants & Statics

    /// <summary>
    /// Gets CancellationToken or None.
    /// </summary>
    public static CancellationToken GetCancellationToken(this GrainCancellationToken? token)
    {
        return token?.CancellationToken ?? CancellationToken.None;
    }

    /// <summary>
    /// Registers to.
    /// </summary>
    /// <param name="gcts">The Grain cts.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="CancellationTokenRegistration" /> instance that can be used to unregister the gcts.</returns>
    public static CancellationTokenRegistration RegisterTo(this GrainCancellationTokenSource gcts,
        CancellationToken cancellationToken)
    {
        // Cancel() return Task, if the Cancel() throw Exception no wait no crash!
        return cancellationToken.Register(() => gcts.Cancel());
    }

#pragma warning disable CA1068 // CancellationToken parameters must come last

    /// <summary>
    /// Registers to.
    /// </summary>
    /// <param name="gcts">The Grain cts.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="continuationAction">The Grain cts canceled continuation action.</param>
    /// <returns>The <see cref="CancellationTokenRegistration" /> instance that can be used to unregister the gcts.</returns>

    public static CancellationTokenRegistration RegisterTo(this GrainCancellationTokenSource gcts,
        CancellationToken cancellationToken, Action<Task> continuationAction)

    {
        return cancellationToken.Register(() => gcts.Cancel().ContinueWith(continuationAction));
    }

    #endregion

#pragma warning restore CA1068 // CancellationToken parameters must come last
}
