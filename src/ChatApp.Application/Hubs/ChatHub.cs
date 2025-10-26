using System.Security.Claims;
using ChatApp.Application.Commands.Messages.SendMessage;
<<<<<<< HEAD
=======
using ChatApp.Application.DTOs.Common;
>>>>>>> a957673 (initial)
using ChatApp.Application.Queries.Groups.GetUserGroups;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

// Infrastructure/Hubs/ChatHub.cs

<<<<<<< HEAD
namespace ChatApp.Api.Hubs;
=======
namespace ChatApp.Application.Hubs;
>>>>>>> a957673 (initial)


[Authorize]
public class ChatHub : Hub
{
    private readonly IMediator _mediator;

    public ChatHub(IMediator mediator)
    {
        _mediator = mediator;
    }

<<<<<<< HEAD
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
=======
    private string? GetUserId()
    {
        // Try ClaimTypes.NameIdentifier first
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        // If not found, try alternative claim names
        if (string.IsNullOrEmpty(userId))
        {
            userId = Context.User?.FindFirst("sub")?.Value; // Standard JWT subject claim
        }
        
        if (string.IsNullOrEmpty(userId))
        {
            userId = Context.User?.FindFirst("nameid")?.Value; // Short form
        }
        
        return userId;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
>>>>>>> a957673 (initial)
        if (!string.IsNullOrEmpty(userId))
        {
            // Join user to their personal channel
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);

            // Join user to all their groups
<<<<<<< HEAD
            var userGroups = await _mediator.Send(new GetUserGroupsQuery { UserId = Guid.Parse(userId) });
            foreach (var group in userGroups)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, group.Id.ToString());
=======
            var userGroupsResponse = await _mediator.Send(new GetUserGroupsQuery { UserId = Guid.Parse(userId) });
            if (userGroupsResponse.IsSuccess && userGroupsResponse.Data != null)
            {
                foreach (var group in userGroupsResponse.Data)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, group.Id.ToString());
                }
>>>>>>> a957673 (initial)
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

<<<<<<< HEAD
    public async Task SendMessage(SendMessageRequest request)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
=======
    public async Task<AppResponse<MessageDto>> SendMessage(SendMessageRequest request)
    {
        var userId = GetUserId();
>>>>>>> a957673 (initial)
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

<<<<<<< HEAD
        await _mediator.Send(command);
=======
        return await _mediator.Send(command);
>>>>>>> a957673 (initial)
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