namespace CleanArchitecture.Blazor.Application.Common.Models;

public class PaginatedData<T>
{
    public PaginatedData(IEnumerable<T> items, int total, int pageIndex, int pageSize)
    {
        Items       = items;
        TotalItems  = total;
        CurrentPage = pageIndex;
        TotalPages  = (int)Math.Ceiling(total / (double)pageSize);
    }

    public int            CurrentPage     { get; }
    public int            TotalItems      { get; private set; }
    public int            TotalPages      { get; }
    public bool           HasPreviousPage => CurrentPage > 1;
    public bool           HasNextPage     => CurrentPage < TotalPages;
    public IEnumerable<T> Items           { get; set; }

    public static async Task<PaginatedData<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
        int count = await source.CountAsync();
        List<T> items = await source.Skip((pageIndex - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();
        return new PaginatedData<T>(items, count, pageIndex, pageSize);
    }
}