using System.Globalization;
using Blazor.Server.UI.Services.UserPreferences;
using Microsoft.IdentityModel.Tokens;

namespace Blazor.Server.UI.Services.Layout;

public class LayoutService
{
    private readonly IUserPreferencesService         _userPreferencesService;
    private          bool                            _systemPreferences;
    private          UserPreferences.UserPreferences _userPreferences = new UserPreferences.UserPreferences();
    public           DarkLightMode                   DarkModeToggle   = DarkLightMode.System;

    public LayoutService(IUserPreferencesService userPreferencesService)
    {
        _userPreferencesService = userPreferencesService;
    }

    public bool     IsRtl           { get; private set; }
    public bool     IsDarkMode      { get; private set; }
    public string   PrimaryColor    { get; set; }         = "#2d4275";
    public string   SecondaryColor  { get; set; }         = "#ff4081ff";
    public double   BorderRadius    { get; set; }         = 4;
    public double   DefaultFontSize { get; set; }         = 0.8125;
    public MudTheme CurrentTheme    { get; private set; } = new MudTheme();

    public void SetDarkMode(bool value)
    {
        IsDarkMode = value;
    }

    public async Task<UserPreferences.UserPreferences> ApplyUserPreferences(bool isDarkModeDefaultTheme)
    {
        _userPreferences = await _userPreferencesService.LoadUserPreferences();
        if (_userPreferences != null)
        {
            IsDarkMode = _userPreferences.DarkLightTheme switch
                         {
                             DarkLightMode.Dark => true,
                             DarkLightMode.Light => false,
                             DarkLightMode.System => isDarkModeDefaultTheme,
                             _ => IsDarkMode
                         };
            IsRtl                                             = _userPreferences.RightToLeft;
            PrimaryColor                                      = _userPreferences.PrimaryColor;
            BorderRadius                                      = _userPreferences.BorderRadius;
            DefaultFontSize                                   = _userPreferences.DefaultFontSize;
            CurrentTheme.Palette.Primary                      = PrimaryColor;
            CurrentTheme.PaletteDark.Primary                  = PrimaryColor;
            CurrentTheme.LayoutProperties.DefaultBorderRadius = BorderRadius                                                     + "px";
            CurrentTheme.Typography.Default.FontSize          = DefaultFontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem";
        }
        else
        {
            IsDarkMode       = isDarkModeDefaultTheme;
            _userPreferences = new UserPreferences.UserPreferences { IsDarkMode = IsDarkMode };
            await _userPreferencesService.SaveUserPreferences(_userPreferences);
        }

        return _userPreferences;
    }

    public event EventHandler? MajorUpdateOccured;

    private void OnMajorUpdateOccured()
    {
        MajorUpdateOccured?.Invoke(this, EventArgs.Empty);
    }

    public Task OnSystemPreferenceChanged(bool newValue)
    {
        _systemPreferences = newValue;
        if (DarkModeToggle == DarkLightMode.System)
        {
            IsDarkMode = newValue;
            OnMajorUpdateOccured();
        }

        return Task.CompletedTask;
    }

    public async Task ToggleDarkMode()
    {
        switch (DarkModeToggle)
        {
            case DarkLightMode.System:
                DarkModeToggle = DarkLightMode.Light;
                IsDarkMode     = false;
                break;
            case DarkLightMode.Light:
                DarkModeToggle = DarkLightMode.Dark;
                IsDarkMode     = true;
                break;
            case DarkLightMode.Dark:
                DarkModeToggle = DarkLightMode.System;
                IsDarkMode     = _systemPreferences;
                break;
        }

        _userPreferences.DarkLightTheme = DarkModeToggle;
        await _userPreferencesService.SaveUserPreferences(_userPreferences);
        OnMajorUpdateOccured();
    }

    public async Task ToggleRightToLeft()
    {
        IsRtl                        = !IsRtl;
        _userPreferences.RightToLeft = IsRtl;
        await _userPreferencesService.SaveUserPreferences(_userPreferences);
        OnMajorUpdateOccured();
    }

    public async Task SetRightToLeft()
    {
        if (!IsRtl)
        {
            await ToggleRightToLeft();
        }
    }

    public async Task SetLeftToRight()
    {
        if (IsRtl)
        {
            await ToggleRightToLeft();
        }
    }

    public void SetBaseTheme(MudTheme theme)
    {
        CurrentTheme = theme;

        if (!PrimaryColor.IsNullOrEmpty())
        {
            CurrentTheme.Palette.Primary     = PrimaryColor;
            CurrentTheme.PaletteDark.Primary = PrimaryColor;
        }

        CurrentTheme.LayoutProperties.DefaultBorderRadius = BorderRadius                                                     + "px";
        CurrentTheme.Typography.Default.FontSize          = DefaultFontSize.ToString("0.0000", CultureInfo.InvariantCulture) + "rem"; //Added
        OnMajorUpdateOccured();
    }

    public async Task SetPrimaryColor(string color)
    {
        PrimaryColor                     = color;
        CurrentTheme.Palette.Primary     = PrimaryColor;
        CurrentTheme.PaletteDark.Primary = PrimaryColor;
        _userPreferences.PrimaryColor    = PrimaryColor;
        await _userPreferencesService.SaveUserPreferences(_userPreferences);
        OnMajorUpdateOccured();
    }

    public async Task SetSecondaryColor(string color)
    {
        SecondaryColor                     = color;
        CurrentTheme.Palette.Secondary     = SecondaryColor;
        CurrentTheme.PaletteDark.Secondary = SecondaryColor;
        _userPreferences.SecondaryColor    = SecondaryColor;
        await _userPreferencesService.SaveUserPreferences(_userPreferences);
        OnMajorUpdateOccured();
    }

    public async Task SetBorderRadius(double size)
    {
        BorderRadius                                      = size;
        CurrentTheme.LayoutProperties.DefaultBorderRadius = BorderRadius + "px";
        _userPreferences.BorderRadius                     = BorderRadius;
        await _userPreferencesService.SaveUserPreferences(_userPreferences);
        OnMajorUpdateOccured();
    }

    public async Task UpdateUserPreferences(UserPreferences.UserPreferences preferences)
    {
        _userPreferences = preferences;
        IsDarkMode = _userPreferences.DarkLightTheme switch
                     {
                         DarkLightMode.Dark => true,
                         DarkLightMode.Light => false,
                         DarkLightMode.System => _systemPreferences = true,
                         _ => IsDarkMode
                     };
        IsRtl           = _userPreferences.RightToLeft;
        PrimaryColor    = _userPreferences.PrimaryColor;
        BorderRadius    = _userPreferences.BorderRadius;
        DefaultFontSize = _userPreferences.DefaultFontSize;
        DarkModeToggle  = _userPreferences.DarkLightTheme;
        await _userPreferencesService.SaveUserPreferences(_userPreferences);
        SetBaseTheme(Theme.Theme.ApplicationTheme());
    }
}