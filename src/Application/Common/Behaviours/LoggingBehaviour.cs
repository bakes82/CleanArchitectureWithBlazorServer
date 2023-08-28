namespace CleanArchitecture.Blazor.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger             _logger;

    public LoggingBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService)
    {
        _logger             = logger;
        _currentUserService = currentUserService;
    }

    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        string  requestName = nameof(TRequest);
        string? userName    = _currentUserService.UserName;
        _logger.LogTrace("Request: {Name} with {@Request} by {@UserName}", requestName, request, userName);
        return Task.CompletedTask;
    }
}