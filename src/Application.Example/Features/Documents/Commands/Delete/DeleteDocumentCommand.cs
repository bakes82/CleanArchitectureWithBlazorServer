using Application.Example.Features.Documents.Caching;

namespace Application.Example.Features.Documents.Commands.Delete;

public class DeleteDocumentCommand : ICacheInvalidatorRequest<Result<int>>
{
    public DeleteDocumentCommand(int[] id)
    {
        Id = id;
    }

    public int[]                    Id                      { get; set; }
    public CancellationTokenSource? SharedExpiryTokenSource => DocumentCacheKey.SharedExpiryTokenSource();
}

public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, Result<int>>

{
    private readonly IApplicationDbContext _context;

    public DeleteDocumentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        List<Document> items = await _context.Documents.Where(x => request.Id.Contains(x.Id))
                                             .ToListAsync(cancellationToken);
        foreach (Document item in items)
        {
            item.AddDomainEvent(new DeletedEvent<Document>(item));
            _context.Documents.Remove(item);
        }

        int result = await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(result);
    }
}