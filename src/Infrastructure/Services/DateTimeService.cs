namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime Now => DateTime.UtcNow;
    public DateTime UtcNow => DateTime.UtcNow;

    public DateTimeOffset DateTimeOffsetNow => DateTimeOffset.UtcNow;
    public DateTimeOffset DateTimeOffsetUtcNow => DateTimeOffset.UtcNow;
}