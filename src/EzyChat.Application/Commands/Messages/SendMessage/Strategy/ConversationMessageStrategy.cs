using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Hubs;
using Microsoft.AspNetCore.SignalR;
namespace EzyChat.Application.Commands.Messages.SendMessage.Strategy;

public class ConversationMessageStrategy(
    IRepository<Message> messageRepository,
    IRepository<Conversation> conversationRepository,
    IHubContext<ChatHub> hubContext,
    IUserRepository userRepository
) : ISendMessageStrategy
{
    public bool CanHandle(SendMessageCommand command)
    {
        return command.Type == "user" && command.ReceiverId.HasValue;
    }

    public async Task<AppResponse<MessageDto>> SendAsync(SendMessageCommand command, CancellationToken cancellationToken)
    {
        var message = command.Adapt<Message>();
        var isNewConversation = command is { Type: "user", ConversationId: null };
        var senderFullName = "";
        
        if (isNewConversation)
        {
            var conversation = new Conversation
            {
                SenderId = command.SenderId,
                ReceiverId = command.ReceiverId ?? Guid.Empty,
                LastMessageAt = DateTime.Now
            };
            await conversationRepository.AddAsync(conversation, cancellationToken);
            message.ConversationId = conversation.Id;
            var senderUser = await userRepository.GetByIdAsync(command.SenderId, cancellationToken: cancellationToken);
            senderFullName = senderUser?.GetFullName();
        }
        else
        {
            message.ConversationId = command.ConversationId;
        }

        await messageRepository.AddAsync(message, cancellationToken);
        
        var messageDto = message.Adapt<MessageDto>();
        
        messageDto.ReceiverId = command.ReceiverId ?? Guid.Empty;
        messageDto.IsNewConversation = isNewConversation;
        messageDto.SenderFullName = senderFullName ?? string.Empty;

        // Send via SignalR to the specific user
        await hubContext.Clients.User(command.ReceiverId!.Value.ToString())
            .SendAsync("OnReceiveMessage", messageDto, cancellationToken);
        
        return AppResponse<MessageDto>.Success(messageDto);
    }

}