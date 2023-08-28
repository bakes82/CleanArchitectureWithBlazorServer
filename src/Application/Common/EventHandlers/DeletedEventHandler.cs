//using CleanArchitecture.Blazor.Domain.Common;

//namespace CleanArchitecture.Blazor.Application.Common.EventHandlers;
//public class DeletedEventHandler<T> : INotificationHandler<DomainEventNotification<DeletedEvent<T>>> where T : IHasDomainEvent
//{
//    private readonly ILogger<DeletedEventHandler<T>> _logger;
//    public DeletedEventHandler(ILogger<DeletedEventHandler<T>> logger)
//    {
//        _logger = logger;
//    }
//    public Task Handle(DomainEventNotification<DeletedEvent<T>> notification, CancellationToken cancellationToken)
//    {
//        var domainEvent = notification.DomainEvent;
//        _logger.LogInformation("Domain Event: {DomainEvent}", domainEvent.GetType().Name);
//        return Task.CompletedTask;
//    }
//}

