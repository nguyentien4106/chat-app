using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ChatApp.Application.Commands.Messages.SendMessage;

public class SendMessageHandler(
    IRepository<Message> messageRepository,
    IHubContext<ChatHub> hubContext,
    ILogger<SendMessageHandler> logger,
    IRepository<Conversation> conversationRepository
) : ICommandHandler<SendMessageCommand, AppResponse<MessageDto>>
{
    public async Task<AppResponse<MessageDto>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        // 1. Get or create conversation first (only for direct messages, not groups)
        Conversation? conversation = null;
        if (request.ReceiverId.HasValue && !request.GroupId.HasValue)
        {
            conversation = await GetOrCreateConversationAsync(
                request.SenderId, 
                request.ReceiverId.Value, 
                cancellationToken);
        }

        // 2. Create and save message
        var message = request.Adapt<Message>();
        await messageRepository.AddAsync(message, cancellationToken);

        // 3. Update conversation's LastMessageAt (only for direct messages)
        if (conversation != null)
        {
            conversation.LastMessageAt = message.CreatedAt;
            await conversationRepository.UpdateAsync(conversation, cancellationToken);
        }
        
        // Load sender info
        logger.LogInformation("Message sent: {MessageId} from user: {SenderId}", message.Id, request.SenderId);
        var sender = message.Sender;

        var messageDto = message.Adapt<MessageDto>();

        // Send via SignalR
        if (request.GroupId.HasValue)
        {
            await hubContext.Clients.Group(request.GroupId.Value.ToString())
                .SendAsync("ReceiveMessage", messageDto, cancellationToken);
        }
        else if (request.ReceiverId.HasValue)
        {
            await hubContext.Clients.User(request.ReceiverId.Value.ToString())
                .SendAsync("ReceiveMessage", messageDto, cancellationToken);
        }

        return AppResponse<MessageDto>.Success(messageDto);
    }

    private async Task<Conversation> GetOrCreateConversationAsync(
        Guid senderId, 
        Guid receiverId, 
        CancellationToken cancellationToken)
    {
        // Order user IDs consistently to avoid duplicates
        var (user1Id, user2Id) = senderId.CompareTo(receiverId) < 0 
            ? (senderId, receiverId) 
            : (receiverId, senderId);

        // Single query instead of parallel queries to avoid DbContext concurrency
        var conversation = await conversationRepository.GetSingleAsync(
            c => c.User1Id == user1Id && c.User2Id == user2Id,
            cancellationToken: cancellationToken);

        if (conversation == null)
        {
            conversation = new Conversation
            {
                User1Id = user1Id,
                User2Id = user2Id,
                LastMessageAt = DateTime.UtcNow
            };
            await conversationRepository.AddAsync(conversation, cancellationToken);
        }

        return conversation;
    }
}