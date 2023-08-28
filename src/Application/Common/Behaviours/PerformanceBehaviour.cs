using System.Diagnostics;

namespace CleanArchitecture.Blazor.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<TRequest>   _logger;
    private readonly Stopwatch           _timer;

    public PerformanceBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService)
    {
        _timer = new Stopwatch();

        _logger             = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        TResponse response = await next()
            .ConfigureAwait(false);
        ;

        _timer.Stop();

        long elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            string requestName = typeof(TRequest).Name;

            string? userName = _currentUserService.UserName;
            _logger.LogWarning("{Name} long running request ({ElapsedMilliseconds} milliseconds) with {@Request} {@UserName} ", requestName, elapsedMilliseconds, request, userName);
        }

        return response;
    }
}