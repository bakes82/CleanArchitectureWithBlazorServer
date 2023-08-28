namespace Application.Example.Features.Customers.Caching;

public static class CustomerCacheKey
{
    public const            string                  GetAllCacheKey  = "all-Customers";
    private static readonly TimeSpan                RefreshInterval = TimeSpan.FromHours(3);
    private static          CancellationTokenSource _tokensource;

    static CustomerCacheKey()
    {
        _tokensource = new CancellationTokenSource(RefreshInterval);
    }

    public static MemoryCacheEntryOptions MemoryCacheEntryOptions =>
        new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource()
                                                                                         .Token));

    public static string GetPaginationCacheKey(string parameters)
    {
        return $"CustomerCacheKey:CustomersWithPaginationQuery,{parameters}";
    }

    public static string GetByNameCacheKey(string parameters)
    {
        return $"CustomerCacheKey:GetByNameCacheKey,{parameters}";
    }

    public static string GetByIdCacheKey(string parameters)
    {
        return $"CustomerCacheKey:GetByIdCacheKey,{parameters}";
    }

    public static CancellationTokenSource SharedExpiryTokenSource()
    {
        if (_tokensource.IsCancellationRequested)
        {
            _tokensource = new CancellationTokenSource(RefreshInterval);
        }

        return _tokensource;
    }

    public static void Refresh()
    {
        SharedExpiryTokenSource()
            .Cancel();
    }
}