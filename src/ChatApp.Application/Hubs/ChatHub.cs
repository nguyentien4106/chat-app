using System.Security.Claims;
using ChatApp.Application.Commands.Messages.SendMessage;
using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Queries.Groups.GetUserGroups;
using ChatApp.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

// Infrastructure/Hubs/ChatHub.cs

namespace ChatApp.Application.Hubs;


[Authorize]
public class ChatHub : Hub
{
    private readonly IMediator _mediator;

    public ChatHub(IMediator mediator)
    {
        _mediator = mediator;
    }

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
        if (!string.IsNullOrEmpty(userId))
        {
            // Join user to their personal channel
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);

            // Join user to all their groups
            var userGroupsResponse = await _mediator.Send(new GetUserGroupsQuery { UserId = Guid.Parse(userId) });
            if (userGroupsResponse.IsSuccess && userGroupsResponse.Data != null)
            {
                foreach (var group in userGroupsResponse.Data)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, group.Id.ToString());
                }
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    public async Task<AppResponse<MessageDto>> SendMessage(SendMessageRequest request)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            throw new HubException("User not authenticated");
        }

        request.SenderId = GetUserId();

        var command = request.Adapt<SendMessageCommand>();

        return await _mediator.Send(command);
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

    public async Task OnNewConversation(Guid groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
    }
}

public class SendMessageRequest
{
    public string? SenderId { get; set; }
    public Guid? ReceiverId { get; set; }
    public Guid? GroupId { get; set; }
    public string? Content { get; set; } = string.Empty;
    
    public MessageTypes MessageType { get; set; }
    
    public string? FileUrl { get; set; }
    
    public string? FileName { get; set; }
    
    public string? FileType { get; set; }
    
    public long FileSize { get; set; }
}