namespace CleanArchitecture.Blazor.Application.Common.Models;

public partial class PaginationFilter : BaseFilter, IHaveDateTimes
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 15;
    public string OrderBy { get; set; } = "Id";
    public string SortDirection { get; set; } = "Descending";
    public override string ToString() => $"PageNumber:{PageNumber},PageSize:{PageSize},OrderBy:{OrderBy},SortDirection:{SortDirection},Keyword:{Keyword}";
    public string? TimeZone { get; set; }
}

public class BaseFilter
{
    public string? Keyword { get; set; }
}

