using CleanArchitecture.Blazor.Domain.Entities.Logger;

namespace CleanArchitecture.Blazor.Application.Features.Loggers.Specifications;
#nullable disable warnings
public sealed class LoggerAdvancedSpecification : Specification<Logger>
{
    public LoggerAdvancedSpecification(LoggerAdvancedFilter filter)
    {
        DateTime today = DateTime.Now.ToUniversalTime()
                                 .Date;
        DateTime last30days = Convert.ToDateTime(today.AddDays(-30)
                                                      .ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) +
                                                 " 00:00:00", CultureInfo.CurrentCulture);
        Query.Where(p => p.TimeStamp.Date == DateTime.Now.Date, filter.ListView == LogListView.CreatedToday)
             .Where(p => p.TimeStamp      >= last30days, filter.ListView        == LogListView.Last30days)
             .Where(p => p.Level          == filter.Level.ToString(), filter.Level is not null)
             .Where(x => x.Message.Contains(filter.Keyword) || x.Exception.Contains(filter.Keyword) || x.UserName.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword));
    }
}