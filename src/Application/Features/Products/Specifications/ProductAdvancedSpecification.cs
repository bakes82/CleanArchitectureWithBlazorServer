
using MMC.AzureAdminBlazorServerUi.Application.Common.Extensions;

namespace CleanArchitecture.Blazor.Application.Features.Products.Specifications;
#nullable disable warnings
public class ProductAdvancedSpecification : Specification<Product>
{
    public ProductAdvancedSpecification(ProductAdvancedFilter filter)
    {
        var      timezone              = filter.TimeZone ?? "UTC";
        DateTime todayInClientTimeZone = DateTime.UtcNow.ConvertUtcToTimeZone(timezone).Date;
        DateTime start                 = todayInClientTimeZone;
        DateTime end                   = todayInClientTimeZone.AddDays(1).AddTicks(-1);
        DateTime last30day             = todayInClientTimeZone.AddDays(-30);
        
        Console.WriteLine($"Today in client time zone {timezone} is {todayInClientTimeZone}");
        Console.WriteLine($"Start: {start}, End: {end}, Last30Day: {last30day}, UTC NOW: {DateTime.UtcNow} NOW: {DateTime.Now}");
        
        if(filter.TimeZone != null) 
        {
            start     = TimeZoneInfo.ConvertTimeToUtc(start);
            end       = TimeZoneInfo.ConvertTimeToUtc(end);
            last30day = TimeZoneInfo.ConvertTimeToUtc(last30day);
            Console.WriteLine($"Start: {start}, End: {end}, Last30Day: {last30day}");
        }
        
        Query.Where(x => x.Name != null)
             .Where(x => EF.Functions.Like(x.Name, $"%{filter.Keyword}%") || EF.Functions.Like(x.Description, $"%{filter.Keyword}%") || EF.Functions.Like(x.Brand, $"%{filter.Keyword}%"), !string.IsNullOrEmpty(filter.Keyword))
             .Where(x => EF.Functions.Like(x.Name, $"%{filter.Keyword}%"), !string.IsNullOrEmpty(filter.Name))
             .Where(x => x.Unit == filter.Unit, !string.IsNullOrEmpty(filter.Unit))
             .Where(x => x.Brand == filter.Brand, !string.IsNullOrEmpty(filter.Brand))
             .Where(x => x.Price <= filter.MaxPrice, !string.IsNullOrEmpty(filter.Brand))
             .Where(x => x.Price >= filter.MinPrice, filter.MinPrice is not null)
             .Where(x => x.CreatedBy == filter.CurrentUser.UserId, filter.ListView == ProductListView.My)
             .Where(x => x.Created >= start && x.Created <= end, filter.ListView == ProductListView.CreatedToday)
             .Where(x => x.Created >= last30day, filter.ListView == ProductListView.Created30Days);
    }
}
