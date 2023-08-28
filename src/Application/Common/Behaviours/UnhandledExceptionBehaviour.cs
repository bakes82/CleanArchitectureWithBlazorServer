namespace CleanArchitecture.Blazor.Application.Common.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<TRequest>   _logger;

    public UnhandledExceptionBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService)
    {
        _logger             = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next()
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            string  requestName = typeof(TRequest).Name;
            string? userName    = _currentUserService.UserName;
            _logger.LogError(ex, "{Name}: {Exception} with {@Request} by {@UserName}", requestName, ex.Message, request, userName);
            throw;
        }
    }
}