using System.Security.Claims;
using System.Collections.Concurrent;
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
    // Track which users are in which groups: groupId -> HashSet<userId>
    private static readonly ConcurrentDictionary<string, HashSet<string>> GroupConnections = new();
    
    // Track user's connection IDs: userId -> HashSet<connectionId>
    private static readonly ConcurrentDictionary<string, HashSet<string>> UserConnections = new();
    
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
            // Track user connection
            UserConnections.AddOrUpdate(
                userId,
                new HashSet<string> { Context.ConnectionId },
                (key, existing) =>
                {
                    lock (existing)
                    {
                        existing.Add(Context.ConnectionId);
                        return existing;
                    }
                });
            
            // Join user to their personal channel
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);

            // Join user to all their groups
            var groups = await groupRepository.GetAllAsync(g => g.Members.Any(m => m.UserId == Guid.Parse(userId)));   
            foreach (var group in groups)
            {
                var groupIdStr = group.Id.ToString();
                await Groups.AddToGroupAsync(Context.ConnectionId, groupIdStr);
                
                // Track group connection
                GroupConnections.AddOrUpdate(
                    groupIdStr,
                    new HashSet<string> { userId },
                    (key, existing) =>
                    {
                        lock (existing)
                        {
                            existing.Add(userId);
                            return existing;
                        }
                    });
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            // Remove connection from user connections
            if (UserConnections.TryGetValue(userId, out var connections))
            {
                lock (connections)
                {
                    connections.Remove(Context.ConnectionId);
                    if (connections.Count == 0)
                    {
                        UserConnections.TryRemove(userId, out _);
                        
                        // Remove user from all groups if they have no more connections
                        foreach (var groupEntry in GroupConnections)
                        {
                            lock (groupEntry.Value)
                            {
                                groupEntry.Value.Remove(userId);
                                if (groupEntry.Value.Count == 0)
                                {
                                    GroupConnections.TryRemove(groupEntry.Key, out _);
                                }
                            }
                        }
                    }
                }
            }
        }
        
        logger.LogInformation($"User ({userId}) disconnected");   
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
    
    /// <summary>
    /// Remove a user from a group by their userId (removes all their connections from that group)
    /// </summary>
    public async Task RemoveUserFromGroup(Guid groupId, string userId)
    {
        var groupIdStr = groupId.ToString();
        
        // Get all connection IDs for this user
        if (UserConnections.TryGetValue(userId, out var connectionIds))
        {
            // Remove each connection from the SignalR group
            foreach (var connectionId in connectionIds.ToList())
            {
                await Groups.RemoveFromGroupAsync(connectionId, groupIdStr);
            }
        }
        
        // Remove user from group connections tracking
        if (GroupConnections.TryGetValue(groupIdStr, out var users))
        {
            users.Remove(userId);
            if (users.Count == 0)
            {
                GroupConnections.TryRemove(groupIdStr, out _);
            }
        }
        
        logger.LogInformation($"Removed user {userId} from group {groupIdStr}");
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