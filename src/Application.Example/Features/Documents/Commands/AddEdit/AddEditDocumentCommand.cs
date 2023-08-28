using Application.Example.Features.Documents.Caching;
using Application.Example.Features.Documents.DTOs;

namespace Application.Example.Features.Documents.Commands.AddEdit;

public class AddEditDocumentCommand : ICacheInvalidatorRequest<Result<int>>
{
    [Description("Id")]
    public int Id { get; set; }

    [Description("Title")]
    public string? Title { get; set; }

    [Description("Description")]
    public string? Description { get; set; }

    [Description("Is Public")]
    public bool IsPublic { get; set; }

    [Description("URL")]
    public string? Url { get; set; }

    [Description("Document Type")]
    public DocumentType DocumentType { get; set; } = DocumentType.Document;

    [Description("Tenant Id")]
    public string? TenantId { get; set; }

    [Description("Tenant Name")]
    public string? TenantName { get; set; }

    [Description("Status")]
    public JobStatus Status { get; set; } = JobStatus.NotStart;

    [Description("Content")]
    public string? Content { get; set; }

    public UploadRequest? UploadRequest { get; set; }

    public CancellationTokenSource? SharedExpiryTokenSource => DocumentCacheKey.SharedExpiryTokenSource();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<AddEditDocumentCommand, DocumentDto>(MemberList.None)
                .ReverseMap();
        }
    }
}

public class AddEditDocumentCommandHandler : IRequestHandler<AddEditDocumentCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper               _mapper;
    private readonly IUploadService        _uploadService;

    public AddEditDocumentCommandHandler(IApplicationDbContext context, IMapper mapper, IUploadService uploadService)
    {
        _context       = context;
        _mapper        = mapper;
        _uploadService = uploadService;
    }

    public async Task<Result<int>> Handle(AddEditDocumentCommand request, CancellationToken cancellationToken)
    {
        DocumentDto? dto = _mapper.Map<DocumentDto>(request);
        if (request.Id > 0)
        {
            Document? document = await _context.Documents.FindAsync(new object[]
                                                                    {
                                                                        request.Id
                                                                    }, cancellationToken);
            _ = document ?? throw new NotFoundException($"Document {request.Id} Not Found.");
            document.AddDomainEvent(new UpdatedEvent<Document>(document));
            if (request.UploadRequest != null)
            {
                document.Url = await _uploadService.UploadAsync(request.UploadRequest);
            }

            document.Title        = request.Title;
            document.Description  = request.Description;
            document.IsPublic     = request.IsPublic;
            document.DocumentType = request.DocumentType;
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(document.Id);
        }
        else
        {
            Document? document = _mapper.Map<Document>(dto);
            if (request.UploadRequest != null)
            {
                document.Url = await _uploadService.UploadAsync(request.UploadRequest);
            }

            document.AddDomainEvent(new CreatedEvent<Document>(document));
            _context.Documents.Add(document);
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(document.Id);
        }
    }
}