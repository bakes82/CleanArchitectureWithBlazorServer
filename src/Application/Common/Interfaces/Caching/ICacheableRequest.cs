namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;

public interface ICacheableRequest<TResponse> : IRequest<TResponse>
{
    string                   CacheKey => String.Empty;
    MemoryCacheEntryOptions? Options  { get; }
}