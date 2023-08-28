//using CleanArchitecture.Blazor.Domain.Common;

//namespace CleanArchitecture.Blazor.Application.Common.EventHandlers;
//public class CreatedEventHandler<T> : INotificationHandler<DomainEventNotification<CreatedEvent<T>>> where T : IHasDomainEvent
//{
//    private readonly ILogger<CreatedEventHandler<T>> _logger;

//    public CreatedEventHandler(ILogger<CreatedEventHandler<T>> logger)
//    {
//        _logger = logger;
//    }
//    public Task Handle(DomainEventNotification<CreatedEvent<T>> notification, CancellationToken cancellationToken)
//    {
//        var domainEvent = notification.DomainEvent;
//        _logger.LogInformation("Domain Event: {DomainEvent}", domainEvent.GetType().Name);
//        return Task.CompletedTask;
//    }
//}

