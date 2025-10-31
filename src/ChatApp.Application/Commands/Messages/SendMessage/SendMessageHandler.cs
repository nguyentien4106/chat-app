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
        var message = request.Adapt<Message>();
        Conversation conversation;
        // // If it's a direct message (not a group message), get or create conversation
        // if (request is { ReceiverId: not null, GroupId: null })
        // {
        //     conversation = await GetOrCreateConversationAsync(
        //         request.SenderId, 
        //         request.ReceiverId.Value, 
        //         cancellationToken);
        //     
        //     message.ConversationId = conversation.Id;
        //     
        //     // Update conversation's last message timestamp
        //     conversation.LastMessageAt = DateTime.UtcNow;
        //     await conversationRepository.UpdateAsync(conversation, cancellationToken);
        // }
        if (request.ConversationId.HasValue)
        {
            conversation = await conversationRepository.GetByIdAsync(request.ConversationId.Value, cancellationToken: cancellationToken);
        }
        else
        {
            conversation = await GetOrCreateConversationAsync(
            request.SenderId, 
            request.ReceiverId.Value, 
            cancellationToken);
        }
        
        await messageRepository.AddAsync(message, cancellationToken);

        // Load sender info
        logger.LogInformation("Message sent: {MessageId} from user: {SenderId}", message.Id, request.SenderId);

        var messageDto = message.Adapt<MessageDto>();

        // Send via SignalR
        if (request.GroupId.HasValue)
        {
            await hubContext.Clients.Group(request.GroupId.Value.ToString())
                .SendAsync("ReceiveMessage", messageDto, cancellationToken);
        }
        else if (request.ConversationId.HasValue)
        {
            await hubContext.Clients.User(request.ConversationId.Value.ToString())
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