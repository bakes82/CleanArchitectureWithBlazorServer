namespace Application.Example.Features.Documents.EventHandlers;

public class DocumentDeletedEventHandler : INotificationHandler<DeletedEvent<Document>>
{
    private readonly ILogger<DocumentDeletedEventHandler> _logger;

    public DocumentDeletedEventHandler(ILogger<DocumentDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DeletedEvent<Document> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete file: {FileName}", notification.Entity.Url);
        if (string.IsNullOrEmpty(notification.Entity.Url))
        {
            return Task.CompletedTask;
        }

        string folder     = UploadType.Document.GetDescription();
        string folderName = Path.Combine("Files", folder);
        string deleteFile = Path.Combine(Directory.GetCurrentDirectory(), folderName, notification.Entity.Url);
        if (File.Exists(deleteFile))
        {
            File.Delete(deleteFile);
        }

        return Task.CompletedTask;
    }
}