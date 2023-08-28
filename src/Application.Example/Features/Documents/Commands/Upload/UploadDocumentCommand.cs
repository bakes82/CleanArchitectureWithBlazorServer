using Application.Example.Features.Documents.Caching;

namespace Application.Example.Features.Documents.Commands.Upload;

public class UploadDocumentCommand : ICacheInvalidatorRequest<Result<int>>
{
    public UploadDocumentCommand(List<UploadRequest> uploadRequests)
    {
        UploadRequests = uploadRequests;
    }

    public List<UploadRequest> UploadRequests { get; set; }

    public CancellationTokenSource? SharedExpiryTokenSource => DocumentCacheKey.SharedExpiryTokenSource();
}

public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper               _mapper;
    private readonly IUploadService        _uploadService;

    public UploadDocumentCommandHandler(IApplicationDbContext context, IMapper mapper, IUploadService uploadService)
    {
        _context       = context;
        _mapper        = mapper;
        _uploadService = uploadService;
    }

    public async Task<Result<int>> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        List<Document> list = new List<Document>();
        foreach (UploadRequest uploadRequest in request.UploadRequests)
        {
            string fileName = uploadRequest.FileName;
            string url      = await _uploadService.UploadAsync(uploadRequest);
            Document document = new Document
                                {
                                    Title        = fileName,
                                    Url          = url,
                                    Status       = JobStatus.Queueing,
                                    IsPublic     = true,
                                    DocumentType = DocumentType.Image
                                };
            document.AddDomainEvent(new CreatedEvent<Document>(document));
            list.Add(document);
        }

        if (!list.Any())
        {
            return await Result<int>.SuccessAsync(0);
        }

        await _context.Documents.AddRangeAsync(list, cancellationToken);
        int result = await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(result);
    }
}