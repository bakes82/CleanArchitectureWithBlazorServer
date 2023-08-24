using Microsoft.Extensions.Localization;
using TimeZoneConverter;

namespace Blazor.Server.UI.Components.Common;

public class TimezonesEnumerableStringsAutocomplete : EnumerableStringsAutocomplete
{
    [Parameter]
    public TimezoneFormats TimezonesFormat { get; set; } = TimezoneFormats.Iana;
    
    [Inject]
    public IStringLocalizer<TimezonesEnumerableStringsAutocomplete> Localizer { get; set; }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        ListOfValues = TimezonesFormat switch
                       {
                           TimezoneFormats.Iana => TZConvert.KnownIanaTimeZoneNames,
                           _ => TZConvert.KnownWindowsTimeZoneIds
                       };

        Placeholder           = Localizer["Set Timezone"];
        Label                 = Localizer["Set Timezone"];
        ResetValueOnEmptyText = false;
        return base.SetParametersAsync(parameters);
    }
}

public enum TimezoneFormats
{
    Windows,
    Iana
}