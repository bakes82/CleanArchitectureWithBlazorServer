using CleanArchitecture.Blazor.Application.Common.Extensions;
using Microsoft.Extensions.Localization;

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
                           TimezoneFormats.Iana => TimeZoneInfoExtensions.GetIanaTimeZones(),
                           _ => TimeZoneInfoExtensions.GetWindowsTimeZones()
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