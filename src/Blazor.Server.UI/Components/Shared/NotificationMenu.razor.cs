using Blazor.Server.UI.Services.Notifications;

namespace Blazor.Server.UI.Components.Shared;

public partial class NotificationMenu : MudComponentBase
{
    private IDictionary<NotificationMessage, bool>? _messages = null;

    private bool _newNotificationsAvailable = false;

    [Inject]
    public INotificationService NotificationService { get; set; } = null!;

    private async Task MarkNotificationAsRead()
    {
        await NotificationService.MarkNotificationsAsRead();
        _newNotificationsAvailable = false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _newNotificationsAvailable = await NotificationService.AreNewNotificationsAvailable();
            _messages                  = await NotificationService.GetNotifications();
            StateHasChanged();
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}