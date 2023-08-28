using Application.Example.Features.Customers.Caching;
using Application.Example.Features.Customers.DTOs;
using Application.Example.Features.Customers.Specifications;

namespace Application.Example.Features.Customers.Queries.Pagination;

public class CustomersWithPaginationQuery : CustomerAdvancedFilter, ICacheableRequest<PaginatedData<CustomerDto>>
{
    public CustomerAdvancedPaginationSpec Specification => new CustomerAdvancedPaginationSpec(this);
    public string                         CacheKey      => CustomerCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions?       Options       => CustomerCacheKey.MemoryCacheEntryOptions;

    public override string ToString()
    {
        return $"Listview:{ListView}, Search:{Keyword}, {OrderBy}, {SortDirection}, {PageNumber}, {PageSize}";
    }
}

public class CustomersWithPaginationQueryHandler : IRequestHandler<CustomersWithPaginationQuery, PaginatedData<CustomerDto>>
{
    private readonly IApplicationDbContext                                 _context;
    private readonly IStringLocalizer<CustomersWithPaginationQueryHandler> _localizer;
    private readonly IMapper                                               _mapper;

    public CustomersWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper, IStringLocalizer<CustomersWithPaginationQueryHandler> localizer)
    {
        _context   = context;
        _mapper    = mapper;
        _localizer = localizer;
    }

    public async Task<PaginatedData<CustomerDto>> Handle(CustomersWithPaginationQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement CustomersWithPaginationQueryHandler method 
        PaginatedData<CustomerDto> data = await _context.Customers.OrderBy($"{request.OrderBy} {request.SortDirection}")
                                                        .ProjectToPaginatedDataAsync<Customer, CustomerDto>(request.Specification, request.PageNumber, request.PageSize, _mapper.ConfigurationProvider, cancellationToken);
        return data;
    }
}