using CleanArchitecture.Blazor.Application.Services.PaddleOCR;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Example.Features.Documents.EventHandlers;

public class DocumentCreatedEventHandler : INotificationHandler<CreatedEvent<Document>>
{
    private readonly ILogger<DocumentCreatedEventHandler> _logger;
    private readonly IServiceScopeFactory                 _scopeFactory;

    public DocumentCreatedEventHandler(IServiceScopeFactory scopeFactory, ILogger<DocumentCreatedEventHandler> logger)
    {
        _scopeFactory = scopeFactory;
        _logger       = logger;
    }

    public Task Handle(CreatedEvent<Document> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("begin recognition: {Id}", notification.Entity.Id);
        Document domainEvent = notification.Entity;
        int      id          = domainEvent.Id;
        IDocumentOcrJob ocrJob = _scopeFactory.CreateScope()
                                              .ServiceProvider.GetRequiredService<IDocumentOcrJob>();
        BackgroundJob.Enqueue(() => ocrJob.Do(id));
        return Task.CompletedTask;
    }
}