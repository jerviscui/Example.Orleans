using Orleans.GrainDirectory;

namespace UrlShortener;

internal interface IUrlGrain : IGrainWithStringKey
{
    public Task SetUrl(string url);

    public Task<string> GetUrl();
}

[GrainDirectory(DistributedDirectory)]
internal sealed class UrlGrain : Grain, IUrlGrain
{
    public const string DistributedDirectory = "redis";

    /// <inheritdoc />
    public Task SetUrl(string url)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<string> GetUrl()
    {
        throw new NotImplementedException();
    }
}
