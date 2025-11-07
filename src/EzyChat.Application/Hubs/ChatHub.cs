using System.Security.Claims;
using EzyChat.Application.Commands.Messages.SendMessage;
using EzyChat.Application.DTOs.Messages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace EzyChat.Application.Hubs;

[Authorize]
public class ChatHub(
    IMediator mediator,
    ILogger<ChatHub> logger,
    IRepository<Group> groupRepository)
    : Hub
{
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
            var groups = await groupRepository.GetAllAsync(g => g.Members.Any(m => m.UserId == Guid.Parse(userId)));   
            foreach (var group in groups)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, group.Id.ToString());
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation($"User ({GetUserId()}) disconnected");   
        await base.OnDisconnectedAsync(exception);
    }

    public async Task<AppResponse<MessageDto>> SendMessage(SendMessageRequest request)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            throw new HubException("User not authenticated");
        }

        request.SenderId = Guid.Parse(GetUserId() ?? string.Empty);

        var command = request.Adapt<SendMessageCommand>();

        return await mediator.Send(command);
    }

    public async Task JoinGroup(Guid groupId)
    {
        logger.LogInformation($"User ({GetUserId()}) Joining group {groupId}");   
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
    }

    public async Task LeaveGroup(Guid groupId)
    {
        logger.LogInformation($"User ({GetUserId()}) leaving group {groupId}");   
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId.ToString());
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