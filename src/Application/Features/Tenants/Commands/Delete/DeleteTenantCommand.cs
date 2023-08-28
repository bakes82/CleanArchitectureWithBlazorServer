using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Tenants.Commands.Delete;

public class DeleteTenantCommand : ICacheInvalidatorRequest<Result<int>>
{
    public DeleteTenantCommand(string[] id)
    {
        Id = id;
    }

    public string[]                 Id                      { get; }
    public string                   CacheKey                => TenantCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => TenantCacheKey.SharedExpiryTokenSource();
}

public class DeleteTenantCommandHandler : IRequestHandler<DeleteTenantCommand, Result<int>>

{
    private readonly IApplicationDbContext                        _context;
    private readonly IStringLocalizer<DeleteTenantCommandHandler> _localizer;
    private readonly IMapper                                      _mapper;

    public DeleteTenantCommandHandler(IApplicationDbContext context, IStringLocalizer<DeleteTenantCommandHandler> localizer, IMapper mapper)
    {
        _context   = context;
        _localizer = localizer;
        _mapper    = mapper;
    }

    public async Task<Result<int>> Handle(DeleteTenantCommand request, CancellationToken cancellationToken)
    {
        List<Tenant> items = await _context.Tenants.Where(x => request.Id.Contains(x.Id))
                                           .ToListAsync(cancellationToken);
        foreach (Tenant item in items)
        {
            _context.Tenants.Remove(item);
        }

        int result = await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(result);
    }
}