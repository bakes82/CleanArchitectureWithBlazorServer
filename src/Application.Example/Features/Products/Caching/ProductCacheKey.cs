namespace Application.Example.Features.Products.Caching;

public static class ProductCacheKey
{
    public const            string                  GetAllCacheKey  = "all-Products";
    private static readonly TimeSpan                RefreshInterval = TimeSpan.FromHours(1);
    private static          CancellationTokenSource _tokenSource;

    static ProductCacheKey()
    {
        _tokenSource = new CancellationTokenSource(RefreshInterval);
    }

    public static MemoryCacheEntryOptions MemoryCacheEntryOptions =>
        new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource()
                                                                                         .Token));

    public static string GetProductByIdCacheKey(int id)
    {
        return $"GetProductById,{id}";
    }

    public static string GetPaginationCacheKey(string parameters)
    {
        return $"ProductsWithPaginationQuery,{parameters}";
    }

    public static CancellationTokenSource SharedExpiryTokenSource()
    {
        if (_tokenSource.IsCancellationRequested)
        {
            _tokenSource = new CancellationTokenSource(RefreshInterval);
        }

        return _tokenSource;
    }

    public static void Refresh()
    {
        SharedExpiryTokenSource()
            .Cancel();
    }
}