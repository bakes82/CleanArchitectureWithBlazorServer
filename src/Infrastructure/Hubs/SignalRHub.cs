using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CleanArchitecture.Blazor.Infrastructure.Hubs;

public interface ISignalRHub
{
    Task Start(string message);
    Task Completed(string message);
    Task SendMessage(string from, string message);
    Task SendPrivateMessage(string from, string to, string message);
    Task Disconnect(string userId);
    Task Connect(string userId);
    Task SendNotification(string message);
}

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class SignalRHub : Hub<ISignalRHub>
{
    private static readonly ConcurrentDictionary<string, string> OnlineUsers = new ConcurrentDictionary<string, string>();

    public override async Task OnConnectedAsync()
    {
        string id       = Context.ConnectionId;
        string username = Context.User?.Identity?.Name ?? string.Empty;
        if (!OnlineUsers.ContainsKey(id))
        {
            OnlineUsers.TryAdd(id, username);
        }

        await Clients.All.Connect(username);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string id = Context.ConnectionId;
        //try to remove key from dictionary
        if (OnlineUsers.TryRemove(id, out string? username))
        {
            await Clients.All.Disconnect(username);
        }

        await base.OnConnectedAsync();
    }

    public async Task SendMessage(string message)
    {
        string username = Context.User?.Identity?.Name ?? string.Empty;
        await Clients.All.SendMessage(username, message);
    }

    public async Task SendPrivateMessage(string to, string message)
    {
        string username = Context.User?.Identity?.Name ?? string.Empty;
        await Clients.User(to)
                     .SendPrivateMessage(username, to, message);
    }

    public async Task SendNotification(string message)
    {
        await Clients.All.SendNotification(message);
    }

    public async Task Completed(string message)
    {
        await Clients.All.Completed(message);
    }
}