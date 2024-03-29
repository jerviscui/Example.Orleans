using Orleans.Runtime;

namespace UrlShortener;

internal interface IUrlShortenerGrain : IGrainWithStringKey
{
    public Task SetUrl(string url);

    public Task<string> GetUrl();
}

internal sealed class UrlShortenerGrain : Grain, IUrlShortenerGrain
{
    public const string Storage = "urls";

    private readonly IPersistentState<UrlDetails> _cache;

    /// <inheritdoc />
    public UrlShortenerGrain([PersistentState("url", Storage)] IPersistentState<UrlDetails> cache)
    {
        _cache = cache;
    }

    /// <inheritdoc />
    public async Task SetUrl(string url)
    {
        _cache.State = new UrlDetails { Short = this.GetPrimaryKeyString(), FullUrl = url };

        await _cache.WriteStateAsync();
    }

    /// <inheritdoc />
    public Task<string> GetUrl()
    {
        //var cacheState = _cache.State; not null!
        //_cache.RecordExists
        return Task.FromResult(_cache.State.FullUrl);
    }
}

[GenerateSerializer]
internal sealed class UrlDetails
{
    public UrlDetails()
    {
        FullUrl = "";
        Short = "";
    }

    [Id(0)]
    public string FullUrl { get; set; }

    [Id(1)]
    public string Short { get; set; }
}
