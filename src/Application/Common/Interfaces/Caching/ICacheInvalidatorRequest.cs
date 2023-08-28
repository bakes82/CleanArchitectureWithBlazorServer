namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;

public interface ICacheInvalidatorRequest<TResponse> : IRequest<TResponse>
{
    string                   CacheKey                => String.Empty;
    CancellationTokenSource? SharedExpiryTokenSource { get; }
}