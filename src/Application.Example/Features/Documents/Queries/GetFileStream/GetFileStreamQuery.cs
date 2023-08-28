using Application.Example.Features.Documents.Caching;

namespace Application.Example.Features.Documents.Queries.GetFileStream;

public class GetFileStreamQuery : ICacheableRequest<(string, byte[])>
{
    public GetFileStreamQuery(int id)
    {
        Id = id;
    }

    public int Id { get; set; }

    public string                   CacheKey => DocumentCacheKey.GetStreamByIdKey(Id);
    public MemoryCacheEntryOptions? Options  => DocumentCacheKey.MemoryCacheEntryOptions;
}

public class GetFileStreamQueryHandler : IRequestHandler<GetFileStreamQuery, (string, byte[])>
{
    private readonly IApplicationDbContext _context;

    public GetFileStreamQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(string, byte[])> Handle(GetFileStreamQuery request, CancellationToken cancellationToken)
    {
        Document? item = await _context.Documents.FindAsync(new object?[]
                                                            {
                                                                request.Id
                                                            }, cancellationToken);
        if (item is null)
        {
            throw new Exception($"not found document entry by Id:{request.Id}.");
        }

        if (string.IsNullOrEmpty(item.Url))
        {
            return (string.Empty, Array.Empty<byte>());
        }

        string filepath = Path.Combine(Directory.GetCurrentDirectory(), item.Url);
        if (!File.Exists(filepath))
        {
            return (string.Empty, Array.Empty<byte>());
        }

        string fileName = new FileInfo(filepath).Name;
        byte[] buffer   = await File.ReadAllBytesAsync(filepath, cancellationToken);
        return (fileName, buffer);
    }

    internal class DocumentsQuery : Specification<Document>
    {
        public DocumentsQuery(string userId, string tenantId, string keyword)
        {
            Query.Where(p => p.CreatedBy == userId && p.IsPublic == false || p.IsPublic == true)
                 .Where(x => x.TenantId == tenantId, !string.IsNullOrEmpty(tenantId))
                 .Where(x => x.Title!.Contains(keyword) || x.Description!.Contains(keyword), !string.IsNullOrEmpty(keyword));
        }
    }
}