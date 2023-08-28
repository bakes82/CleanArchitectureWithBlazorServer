using Application.Example.Features.Documents.Caching;
using Application.Example.Features.Documents.DTOs;
using Application.Example.Features.Documents.Specifications;

namespace Application.Example.Features.Documents.Queries.PaginationQuery;

public class DocumentsWithPaginationQuery : AdvancedDocumentsFilter, ICacheableRequest<PaginatedData<DocumentDto>>
{
    public AdvancedDocumentsSpecification Specification => new AdvancedDocumentsSpecification(this);

    public string                   CacheKey => DocumentCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options  => DocumentCacheKey.MemoryCacheEntryOptions;

    public override string ToString()
    {
        return $"CurrentUserId:{CurrentUser?.UserId},ListView:{ListView},Search:{Keyword},OrderBy:{OrderBy} {SortDirection},{PageNumber},{PageSize}";
    }
}

public class DocumentsQueryHandler : IRequestHandler<DocumentsWithPaginationQuery, PaginatedData<DocumentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper               _mapper;

    public DocumentsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper  = mapper;
    }

    public async Task<PaginatedData<DocumentDto>> Handle(DocumentsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        PaginatedData<DocumentDto> data = await _context.Documents.OrderBy($"{request.OrderBy} {request.SortDirection}")
                                                        .ProjectToPaginatedDataAsync<Document, DocumentDto>(request.Specification, request.PageNumber, request.PageSize, _mapper.ConfigurationProvider, cancellationToken);

        return data;
    }
}