namespace Application.Example.Features.Customers.Specifications;
#nullable disable warnings
public class CustomerAdvancedPaginationSpec : Specification<Customer>
{
    public CustomerAdvancedPaginationSpec(CustomerAdvancedFilter filter)
    {
        DateTime today = DateTime.Now.ToUniversalTime()
                                 .Date;
        DateTime start = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00", CultureInfo.CurrentCulture);
        DateTime end   = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 23:59:59", CultureInfo.CurrentCulture);
        DateTime last30day = Convert.ToDateTime(today.AddDays(-30)
                                                     .ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) +
                                                " 00:00:00", CultureInfo.CurrentCulture);

        Query.Where(q => q.Name != null)
             .Where(q => q.Name!.Contains(filter.Keyword) || q.Description!.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword))
             .Where(q => q.CreatedBy == filter.CurrentUser.UserId, filter.ListView == CustomerListView.My && filter.CurrentUser is not null)
             .Where(q => q.Created >= start && q.Created <= end, filter.ListView == CustomerListView.CreatedToday)
             .Where(q => q.Created >= last30day, filter.ListView                 == CustomerListView.Created30Days);
    }
}