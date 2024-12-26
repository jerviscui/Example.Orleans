using Orleans;
using Orleans.Runtime;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Grains;

[Alias("Grains.IWatchGrainExtension")]
public interface IWatchGrainExtension : IGrainExtension
{

    #region Methods

    [Alias("Watch")]
    Task WatchAsync(CancellationToken cancellationToken = default);

    #endregion

}

public class WatchGrainExtension : IWatchGrainExtension
{
    private readonly IGrainContext _grainContext;

    public WatchGrainExtension(IGrainContext grainContext)
    {
        _grainContext = grainContext;
    }

    #region IWatchGrainExtension implementations

    public Task WatchAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"{_grainContext.GrainId} {DateTime.Now}");

        return Task.CompletedTask;
    }

    #endregion

}
