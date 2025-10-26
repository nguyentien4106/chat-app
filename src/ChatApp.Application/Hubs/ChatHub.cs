using System.Security.Claims;
using ChatApp.Application.Commands.Messages.SendMessage;
using ChatApp.Application.Queries.Groups.GetUserGroups;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

// Infrastructure/Hubs/ChatHub.cs

namespace ChatApp.Api.Hubs;


[Authorize]
public class ChatHub : Hub
{
    private readonly IMediator _mediator;

    public ChatHub(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            // Join user to their personal channel
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);

            // Join user to all their groups
            var userGroups = await _mediator.Send(new GetUserGroupsQuery { UserId = Guid.Parse(userId) });
            foreach (var group in userGroups)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, group.Id.ToString());
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(SendMessageRequest request)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            throw new HubException("User not authenticated");
        }

        var command = new SendMessageCommand
        {
            SenderId = Guid.Parse(userId),
            ReceiverId = request.ReceiverId,
            GroupId = request.GroupId,
            Content = request.Content
        };

        await _mediator.Send(command);
    }

    public async Task JoinGroup(Guid groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
    }

    public async Task LeaveGroup(Guid groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId.ToString());
    }

    public async Task MarkAsRead(Guid messageId)
    {
        // Implement mark as read logic
        await Clients.Caller.SendAsync("MessageRead", messageId);
    }

    public async Task UserTyping(Guid? receiverId, Guid? groupId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        if (groupId.HasValue)
        {
            await Clients.OthersInGroup(groupId.Value.ToString())
                .SendAsync("UserTyping", new { UserId = userId, GroupId = groupId });
        }
        else if (receiverId.HasValue)
        {
            await Clients.User(receiverId.Value.ToString())
                .SendAsync("UserTyping", new { UserId = userId });
        }
    }
}

public class SendMessageRequest
{
    public Guid? ReceiverId { get; set; }
    public Guid? GroupId { get; set; }
    public string Content { get; set; } = string.Empty;
}