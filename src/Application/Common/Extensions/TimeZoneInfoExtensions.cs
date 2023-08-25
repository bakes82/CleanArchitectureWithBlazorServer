namespace CleanArchitecture.Blazor.Application.Common.Extensions;

public static class TimeZoneInfoExtensions
{
    public static string GetIanaTimeZoneId(this TimeZoneInfo timeZoneInfo)
    {
        if (timeZoneInfo.HasIanaId)
        {
            return timeZoneInfo.Id; // no conversion necessary
        }

        if (TimeZoneInfo.TryConvertWindowsIdToIanaId(timeZoneInfo.Id, out string? ianaId))
        {
            return ianaId; // use the converted ID
        }

        throw new TimeZoneNotFoundException($"No IANA time zone found for \"{timeZoneInfo.Id}\".");
    }

    public static string GetWindowsTimeZoneId(this TimeZoneInfo timeZoneInfo)
    {
        if (!timeZoneInfo.HasIanaId)
        {
            return timeZoneInfo.Id; // no conversion necessary
        }

        if (TimeZoneInfo.TryConvertIanaIdToWindowsId(timeZoneInfo.Id, out string? winId))
        {
            return winId; // use the converted ID
        }

        throw new TimeZoneNotFoundException($"No Windows time zone found for \"{timeZoneInfo.Id}\".");
    }

    public static List<string> GetIanaTimeZones()
    {
        List<string> timeZones = new List<string>();
        foreach (TimeZoneInfo timeZone in TimeZoneInfo.GetSystemTimeZones())
        {
            try
            {
                timeZones.Add(timeZone.GetIanaTimeZoneId());
            }
            catch (TimeZoneNotFoundException)
            {
                // ignored
            }
        }

        return timeZones.OrderBy(t => t)
                        .ToList();
    }

    public static List<string> GetWindowsTimeZones()
    {
        List<string> timeZones = new List<string>();
        foreach (TimeZoneInfo timeZone in TimeZoneInfo.GetSystemTimeZones())
        {
            try
            {
                timeZones.Add(timeZone.GetWindowsTimeZoneId());
            }
            catch (TimeZoneNotFoundException)
            {
                // ignored
            }
        }

        return timeZones.OrderBy(t => t)
                        .ToList();
    }
}