namespace Application.Example.Features.Customers.EventHandlers;

public class CustomerDeletedEventHandler : INotificationHandler<CustomerDeletedEvent>
{
    private readonly ILogger<CustomerDeletedEventHandler> _logger;

    public CustomerDeletedEventHandler(ILogger<CustomerDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(CustomerDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType()
                                                                          .FullName);
        return Task.CompletedTask;
    }
}