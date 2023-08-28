using CleanArchitecture.Blazor.Application.Features.KeyValues.Caching;

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.Delete;

public class DeleteKeyValueCommand : ICacheInvalidatorRequest<Result<int>>
{
    public DeleteKeyValueCommand(int[] id)
    {
        Id = id;
    }

    public int[]                    Id                      { get; }
    public string                   CacheKey                => KeyValueCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => KeyValueCacheKey.SharedExpiryTokenSource();
}

public class DeleteKeyValueCommandHandler : IRequestHandler<DeleteKeyValueCommand, Result<int>>

{
    private readonly IApplicationDbContext _context;

    public DeleteKeyValueCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(DeleteKeyValueCommand request, CancellationToken cancellationToken)
    {
        List<KeyValue> items = await _context.KeyValues.Where(x => request.Id.Contains(x.Id))
                                             .ToListAsync(cancellationToken);
        foreach (KeyValue item in items)
        {
            UpdatedEvent<KeyValue> changeEvent = new UpdatedEvent<KeyValue>(item);
            item.AddDomainEvent(changeEvent);
            _context.KeyValues.Remove(item);
        }

        int result = await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(result);
    }
}