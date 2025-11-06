using EzyChat.Application.Hubs;
using EzyChat.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace EzyChat.Infrastructure.Services;

public class SignalRService(IHubContext<ChatHub> hubContext) : ISignalRService
{
    public async Task NotifyGroupAsync(string connectionId, string action, object data, CancellationToken cancellationToken = default)
    {
        await hubContext.Clients.Group(connectionId).SendAsync(action, data, cancellationToken);
    }

    public async Task NotifyUserAsync(string connectionId, string action, object data, CancellationToken cancellationToken = default)
    {
        await hubContext.Clients.User(connectionId).SendAsync(action, data, cancellationToken);
    }

}