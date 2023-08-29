using Blazor.Server.UI.Components.Shared;
using Blazor.Server.UI.Services.Layout;
using Blazor.Server.UI.Services.UserPreferences;
using Toolbelt.Blazor.HotKeys2;

namespace Blazor.Server.UI.Shared;

public partial class MainLayout : LayoutComponentBase, IDisposable
{
    private bool             _commandPaletteOpen;
    private bool             _defaultDarkMode;
    private HotKeysContext?  _hotKeysContext;
    private MudThemeProvider _mudThemeProvider   = null!;
    private bool             _sideMenuDrawerOpen = true;
    private bool             _themingDrawerOpen;
    private UserPreferences  _userPreferences = new UserPreferences();

    [Inject]
    private LayoutService LayoutService { get; set; } = null!;

    [Inject]
    private HotKeys HotKeys { get; set; } = default!;

    public void Dispose()
    {
        LayoutService.MajorUpdateOccured -= LayoutServiceOnMajorUpdateOccured;
        _hotKeysContext?.Dispose();
        GC.SuppressFinalize(this);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            await ApplyUserPreferences();
            await _mudThemeProvider.WatchSystemPreference(OnSystemPreferenceChanged);
            StateHasChanged();
        }
    }

    private async Task ApplyUserPreferences()
    {
        _defaultDarkMode = await _mudThemeProvider.GetSystemPreference();
        _userPreferences = await LayoutService.ApplyUserPreferences(_defaultDarkMode);
    }

    protected override void OnInitialized()
    {
        LayoutService.MajorUpdateOccured += LayoutServiceOnMajorUpdateOccured;
        LayoutService.SetBaseTheme(Theme.Theme.ApplicationTheme());
        _hotKeysContext = HotKeys.CreateContext()
                                 .Add(ModKey.Ctrl, Key.K, async () => await OpenCommandPalette(), "Open command palette.");
    }

    private async Task OnSystemPreferenceChanged(bool newValue)
    {
        await LayoutService.OnSystemPreferenceChanged(newValue);
    }

    private void LayoutServiceOnMajorUpdateOccured(object? sender, EventArgs e)
    {
        StateHasChanged();
    }

    protected void SideMenuDrawerOpenChangedHandler(bool state)
    {
        _sideMenuDrawerOpen = state;
    }

    protected void ThemingDrawerOpenChangedHandler(bool state)
    {
        _themingDrawerOpen = state;
    }

    protected void ToggleSideMenuDrawer()
    {
        _sideMenuDrawerOpen = !_sideMenuDrawerOpen;
    }

    private async Task OpenCommandPalette()
    {
        if (!_commandPaletteOpen)
        {
            DialogOptions options = new DialogOptions { NoHeader = true, MaxWidth = MaxWidth.Medium, FullWidth = true };

            IDialogReference? commandPalette = await DialogService.ShowAsync<CommandPalette>("", options);
            _commandPaletteOpen = true;

            await commandPalette.Result;
            _commandPaletteOpen = false;
        }
    }
}