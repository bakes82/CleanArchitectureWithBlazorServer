using Blazor.Server.UI.Components.Dialogs;
using CleanArchitecture.Blazor.Application.Features.Fluxor;
using Fluxor;
using Microsoft.AspNetCore.Components.Web;

namespace Blazor.Server.UI.Components.Shared;

public partial class UserMenu : FluxorComponent
{
    [Inject]
    private IState<UserProfileState> UserProfileState { get; set; } = null!;

    private bool        IsLoading   => UserProfileState.Value.IsLoading;
    private UserProfile UserProfile => UserProfileState.Value.UserProfile;

    [Parameter]
    public EventCallback<MouseEventArgs> OnSettingClick { get; set; }

    [Inject]
    protected NavigationManager NavigationManager { get; set; } = null!;

    private async Task OnLogout()
    {
        DialogParameters parameters = new DialogParameters { { nameof(LogoutConfirmation.ContentText), $"{ConstantString.LogoutConfirmation}" }, { nameof(LogoutConfirmation.Color), Color.Error } };

        DialogOptions     options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
        IDialogReference? dialog  = DialogService.Show<LogoutConfirmation>(ConstantString.LogoutConfirmationTitle, parameters, options);
        DialogResult?     result  = await dialog.Result;
        if (!result.Canceled)
        {
            await TokenProvider.RemoveAuthDataFromStorage();
            NavigationManager.NavigateTo("/", true);
        }
    }
}