using CleanArchitecture.Blazor.Application.Features.Loggers.Caching;
using CleanArchitecture.Blazor.Application.Features.Loggers.DTOs;
using CleanArchitecture.Blazor.Application.Features.Loggers.Specifications;
using CleanArchitecture.Blazor.Domain.Entities.Logger;

namespace CleanArchitecture.Blazor.Application.Features.Loggers.Queries.PaginationQuery;

public class LogsWithPaginationQuery : LoggerAdvancedFilter, ICacheableRequest<PaginatedData<LogDto>>
{
    public LoggerAdvancedSpecification Specification => new LoggerAdvancedSpecification(this);

    public string                   CacheKey => LogsCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options  => LogsCacheKey.MemoryCacheEntryOptions;

    public override string ToString()
    {
        return $"Listview:{ListView},{Level},Search:{Keyword},OrderBy:{OrderBy} {SortDirection},{PageNumber},{PageSize}";
    }
}

public class LogsQueryHandler : IRequestHandler<LogsWithPaginationQuery, PaginatedData<LogDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper               _mapper;

    public LogsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper  = mapper;
    }

    public async Task<PaginatedData<LogDto>> Handle(LogsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        PaginatedData<LogDto> data = await _context.Loggers.OrderBy($"{request.OrderBy} {request.SortDirection}")
                                                   .ProjectToPaginatedDataAsync<Logger, LogDto>(request.Specification, request.PageNumber, request.PageSize, _mapper.ConfigurationProvider, cancellationToken);
        return data;
    }
}