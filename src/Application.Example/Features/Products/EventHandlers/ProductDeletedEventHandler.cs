namespace Application.Example.Features.Products.EventHandlers;

public class ProductDeletedEventHandler : INotificationHandler<DeletedEvent<Product>>
{
    private readonly ILogger<ProductDeletedEventHandler> _logger;

    public ProductDeletedEventHandler(ILogger<ProductDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DeletedEvent<Product> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType()
                                                                          .FullName);
        return Task.CompletedTask;
    }
}