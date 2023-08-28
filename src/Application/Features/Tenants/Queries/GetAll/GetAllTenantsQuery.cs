using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Tenants.Queries.GetAll;

public class GetAllTenantsQuery : ICacheableRequest<IEnumerable<TenantDto>>
{
    public string                   CacheKey => TenantCacheKey.GetAllCacheKey;
    public MemoryCacheEntryOptions? Options  => TenantCacheKey.MemoryCacheEntryOptions;
}

public class GetAllTenantsQueryHandler : IRequestHandler<GetAllTenantsQuery, IEnumerable<TenantDto>>
{
    private readonly IApplicationDbContext                       _context;
    private readonly IStringLocalizer<GetAllTenantsQueryHandler> _localizer;
    private readonly IMapper                                     _mapper;

    public GetAllTenantsQueryHandler(IApplicationDbContext context, IMapper mapper, IStringLocalizer<GetAllTenantsQueryHandler> localizer)
    {
        _context   = context;
        _mapper    = mapper;
        _localizer = localizer;
    }

    public async Task<IEnumerable<TenantDto>> Handle(GetAllTenantsQuery request, CancellationToken cancellationToken)
    {
        List<TenantDto> data = await _context.Tenants.OrderBy(x => x.Name)
                                             .ProjectTo<TenantDto>(_mapper.ConfigurationProvider)
                                             .ToListAsync(cancellationToken);
        return data;
    }
}