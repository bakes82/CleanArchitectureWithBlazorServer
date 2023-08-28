namespace CleanArchitecture.Blazor.Application.Features.AuditTrails.Specifications;
#nullable disable warnings
public sealed class AuditTrailAdvancedSpecification : Specification<AuditTrail>
{
    public AuditTrailAdvancedSpecification(AuditTrailAdvancedFilter filter)
    {
        DateTime today = DateTime.Now.ToUniversalTime()
                                 .Date;
        DateTime last30day = Convert.ToDateTime(today.AddDays(-30)
                                                     .ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) +
                                                " 00:00:00", CultureInfo.CurrentCulture);

        Query.Where(p => p.AuditType     == filter.AuditType, filter.AuditType is not null)
             .Where(p => p.UserId        == filter.CurrentUser.UserId, filter.ListView == AuditTrailListView.My && filter.CurrentUser is not null)
             .Where(p => p.DateTime.Date == DateTime.Now.Date, filter.ListView == AuditTrailListView.CreatedToday)
             .Where(p => p.DateTime      >= last30day, filter.ListView         == AuditTrailListView.Last30days)
             .Where(x => x.TableName.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword));
    }
}