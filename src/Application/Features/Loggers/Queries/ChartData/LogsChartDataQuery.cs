using CleanArchitecture.Blazor.Application.Features.Loggers.Caching;
using CleanArchitecture.Blazor.Application.Features.Loggers.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Loggers.Queries.ChartData;

public class LogsTimeLineChartDataQuery : ICacheableRequest<List<LogTimeLineDto>>
{
    public DateTime                 LastDateTime { get; set; } = DateTime.Now.AddDays(-60);
    public string                   CacheKey     => LogsCacheKey.GetChartDataCacheKey(LastDateTime.ToString());
    public MemoryCacheEntryOptions? Options      => LogsCacheKey.MemoryCacheEntryOptions;
}

public class LogsChartDataQueryHandler : IRequestHandler<LogsTimeLineChartDataQuery, List<LogTimeLineDto>>

{
    private readonly IApplicationDbContext                       _context;
    private readonly IStringLocalizer<LogsChartDataQueryHandler> _localizer;

    public LogsChartDataQueryHandler(IApplicationDbContext context, IStringLocalizer<LogsChartDataQueryHandler> localizer)
    {
        _context   = context;
        _localizer = localizer;
    }

    public async Task<List<LogTimeLineDto>> Handle(LogsTimeLineChartDataQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Loggers.Where(x => x.TimeStamp >= request.LastDateTime)
                                 .GroupBy(x => new { x.TimeStamp.Date })
                                 .Select(x => new { x.Key.Date, Total = x.Count() })
                                 .OrderBy(x => x.Date)
                                 .ToListAsync(cancellationToken);

        List<LogTimeLineDto> result = new List<LogTimeLineDto>();
        DateTime             end    = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        DateTime             start  = request.LastDateTime.Date;

        while (start <= end)
        {
            var item = data.FirstOrDefault(x => x.Date == start.Date);
            result.Add(item != null ? new LogTimeLineDto { Dt = item.Date, Total = item.Total } : new LogTimeLineDto { Dt = start, Total = 0 });

            start = start.AddDays(1);
        }

        return result.OrderBy(x => x.Dt)
                     .ToList();
    }
}