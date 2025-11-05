using System.Security.Claims;
using EzyChat.Application.Commands.Messages.SendMessage;
using EzyChat.Application.DTOs.Common;
using EzyChat.Application.Queries.Conversations.GetUserConversations;
using EzyChat.Application.Queries.Groups.GetUserGroups;
using EzyChat.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

// Infrastructure/Hubs/ChatHub.cs

namespace EzyChat.Application.Hubs;


[Authorize]
public class ChatHub : Hub
{
    private readonly IMediator _mediator;
    private readonly IRepository<Conversation> _conversationRepository;
    private readonly IRepository<Group> _groupRepository;
    public ChatHub(IMediator mediator, IRepository<Conversation> conversationRepository, IRepository<Group> groupRepository)
    {
        _mediator = mediator;
        _conversationRepository = conversationRepository;
        _groupRepository = groupRepository;
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
            var groups = await _groupRepository.GetAllAsync(g => g.Members.Any(m => m.UserId == Guid.Parse(userId)));   
            foreach (var group in groups)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, group.Id.ToString());
            }

            // Join user to all their conversations
            var conversations = await _conversationRepository.GetAllAsync(c =>
                c.SenderId == Guid.Parse(userId) || c.ReceiverId == Guid.Parse(userId)
            );
            
            foreach (var conversation in conversations)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, conversation.Id.ToString());
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

        request.SenderId = Guid.Parse(GetUserId() ?? string.Empty);

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
    public Guid SenderId { get; set; }
    
    public Guid? ReceiverId { get; set; }
    public Guid? ConversationId { get; set; }
    public Guid? GroupId { get; set; }
    public string? Content { get; set; } = string.Empty;
    
    public MessageTypes MessageType { get; set; }
    
    public string? FileUrl { get; set; }
    
    public string? FileName { get; set; }
    
    public string? FileType { get; set; }
    
    public long FileSize { get; set; }
}