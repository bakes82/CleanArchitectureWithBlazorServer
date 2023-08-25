namespace CleanArchitecture.Blazor.Application.Common.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ConvertUtcToTimeZone(this DateTime utcDateTime, string timezone, bool checkUtcKind = false)
    {
        if (checkUtcKind && utcDateTime.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("The DateTime must be of DateTimeKind UTC.", nameof(utcDateTime));
        }
        
        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezone);
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZoneInfo);
    }
}