using Application.Example.Features.Products.Caching;
using Application.Example.Features.Products.DTOs;
using Application.Example.Features.Products.Specifications;

namespace Application.Example.Features.Products.Queries.Pagination;

public class ProductsWithPaginationQuery : ProductAdvancedFilter, ICacheableRequest<PaginatedData<ProductDto>>
{
    public ProductAdvancedSpecification Specification => new ProductAdvancedSpecification(this);

    public string CacheKey => ProductCacheKey.GetPaginationCacheKey($"{this}");

    public MemoryCacheEntryOptions? Options => ProductCacheKey.MemoryCacheEntryOptions;

    // the currently logged in user
    public override string ToString()
    {
        return $"CurrentUser:{CurrentUser?.UserId},ListView:{ListView},Search:{Keyword},Name:{Name},Brand:{Brand},Unit:{Unit},MinPrice:{MinPrice},MaxPrice:{MaxPrice},SortDirection:{SortDirection},OrderBy:{OrderBy},{PageNumber},{PageSize}";
    }
}

public class ProductsWithPaginationQueryHandler : IRequestHandler<ProductsWithPaginationQuery, PaginatedData<ProductDto>>
{
    private readonly IApplicationDbContext                                _context;
    private readonly IStringLocalizer<ProductsWithPaginationQueryHandler> _localizer;
    private readonly IMapper                                              _mapper;

    public ProductsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper, IStringLocalizer<ProductsWithPaginationQueryHandler> localizer)
    {
        _context   = context;
        _mapper    = mapper;
        _localizer = localizer;
    }

    public async Task<PaginatedData<ProductDto>> Handle(ProductsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        PaginatedData<ProductDto> data = await _context.Products.OrderBy($"{request.OrderBy} {request.SortDirection}")
                                                       .ProjectToPaginatedDataAsync<Product, ProductDto>(request.Specification, request.PageNumber, request.PageSize, _mapper.ConfigurationProvider, cancellationToken);
        return data;
    }
}