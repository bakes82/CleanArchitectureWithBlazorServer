using System.Collections.Concurrent;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

public class UsersStateContainer : IUsersStateContainer
{
    public ConcurrentDictionary<string, string> UsersByConnectionId { get; } = new ConcurrentDictionary<string, string>();

    public event Action? OnChange;

    public void Update(string connectionId, string? name)
    {
        UsersByConnectionId.AddOrUpdate(connectionId, name ?? String.Empty, (_, _) => name ?? String.Empty);
        NotifyStateChanged();
    }

    public void Remove(string connectionId)
    {
        UsersByConnectionId.TryRemove(connectionId, out string _);
        NotifyStateChanged();
    }

    private void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}