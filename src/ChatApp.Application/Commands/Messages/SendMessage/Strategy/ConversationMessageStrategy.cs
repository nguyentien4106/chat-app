using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Hubs;
using ChatApp.Application.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ChatApp.Application.Commands.Messages.SendMessage.Strategy;

public class ConversationMessageStrategy(
    IRepository<Message> messageRepository,
    IRepository<Conversation> conversationRepository,
    IHubContext<ChatHub> hubContext,
    ILogger<ConversationMessageStrategy> logger
) : ISendMessageStrategy
{
    public bool CanHandle(SendMessageCommand command)
    {
        return !command.GroupId.HasValue;
    }

    public async Task<AppResponse<MessageDto>> SendAsync(SendMessageCommand command, CancellationToken cancellationToken)
    {
        var message = command.Adapt<Message>();
        
        // Get or create conversation
        var conversation = await GetOrCreateConversationAsync(command, cancellationToken);
        message.ConversationId = conversation.Id;
        
        await messageRepository.AddAsync(message, cancellationToken);
        
        // Reload message with Sender navigation property
        message = await messageRepository.GetByIdAsync(
            message.Id, 
            includeProperties: ["Sender"],
            cancellationToken: cancellationToken) ?? message;

        var messageDto = message.Adapt<MessageDto>();
        messageDto.ReceiverId = command.ReceiverId ?? Guid.Empty;
        messageDto.SenderUserName = message.Sender?.UserName ?? string.Empty;
        
        // Send via SignalR to the specific user
        if (command.ReceiverId.HasValue)
        {
            await hubContext.Clients.User(command.ReceiverId.ToString())
                .SendAsync("ReceiveMessage", messageDto, cancellationToken);
        }

        return AppResponse<MessageDto>.Success(messageDto);
    }

    private async Task<Conversation> GetOrCreateConversationAsync(SendMessageCommand command, CancellationToken cancellationToken)
    {
        // If ConversationId is provided, get existing conversation
        if (command.ConversationId.HasValue)
        {
            var existingConversation = await conversationRepository.GetByIdAsync(
                command.ConversationId.Value, 
                includeProperties: ["Sender"],
                cancellationToken: cancellationToken);
            
            if (existingConversation != null)
            {
                return existingConversation;
            }
        }

        // Create or find conversation based on sender and receiver
        if (!command.ReceiverId.HasValue)
        {
            throw new InvalidOperationException("ReceiverId is required for conversation messages when ConversationId is not provided.");
        }

        return await GetOrCreateConversationAsync(
            command.SenderId,
            command.ReceiverId.Value,
            cancellationToken);
    }

    private async Task<Conversation> GetOrCreateConversationAsync(
        Guid senderId, 
        Guid receiverId, 
        CancellationToken cancellationToken)
    {
        // Single query instead of parallel queries to avoid DbContext concurrency
        var conversation = await conversationRepository.GetSingleAsync(
            c => c.SenderId == senderId && c.ReceiverId == receiverId,
            cancellationToken: cancellationToken);

        if (conversation == null)
        {
            conversation = new Conversation
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                LastMessageAt = DateTime.Now
            };
            await conversationRepository.AddAsync(conversation, cancellationToken);
        }

        return conversation;
    }
}