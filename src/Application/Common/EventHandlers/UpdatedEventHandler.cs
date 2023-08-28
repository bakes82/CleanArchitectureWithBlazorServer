//using CleanArchitecture.Blazor.Domain.Common;

//namespace CleanArchitecture.Blazor.Application.Common.EventHandlers;
//public class UpdatedEventHandler<T> : INotificationHandler<DomainEventNotification<UpdatedEvent<T>>> where T : IHasDomainEvent
//{
//    private readonly ILogger<UpdatedEventHandler<T>> _logger;
//    public UpdatedEventHandler(ILogger<UpdatedEventHandler<T>> logger)
//    {
//        _logger = logger;
//    }
//    public Task Handle(DomainEventNotification<UpdatedEvent<T>> notification, CancellationToken cancellationToken)
//    {
//        var domainEvent = notification.DomainEvent;
//        _logger.LogInformation("Domain Event: {DomainEvent}", domainEvent.GetType().Name);
//        return Task.CompletedTask;
//    }
//}

