namespace Application.Example.Features.Products.EventHandlers;

public class ProductUpdatedEventHandler : INotificationHandler<UpdatedEvent<Product>>
{
    private readonly ILogger<ProductUpdatedEventHandler> _logger;

    public ProductUpdatedEventHandler(ILogger<ProductUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(UpdatedEvent<Product> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType()
                                                                          .FullName);

        return Task.CompletedTask;
    }
}