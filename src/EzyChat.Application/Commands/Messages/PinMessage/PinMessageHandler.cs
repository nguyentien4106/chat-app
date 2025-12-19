using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Exceptions;
using EzyChat.Application.Hubs;
using EzyChat.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace EzyChat.Application.Commands.Messages.PinMessage;

public class PinMessageHandler(
    IHubContext<ChatHub> hubContext,
    IRepository<Message> messageRepository,
    UserManager<ApplicationUser> userManager,
    IRepository<Conversation> conversationRepository,
    IRepository<Domain.Entities.PinMessage> repository) 
    : ICommandHandler<PinMessageCommand, AppResponse<PinMessageDto>>
{

    public async Task<AppResponse<PinMessageDto>> Handle(PinMessageCommand request, CancellationToken cancellationToken)
    {
        // Validate that either ConversationId or GroupId is provided, but not both
        if (request.ConversationId == null && request.GroupId == null)
        {
            throw new BadRequestException("Either ConversationId or GroupId must be provided");
        }
        
        if (request.ConversationId != null && request.GroupId != null)
        {
            throw new BadRequestException("Cannot provide both ConversationId and GroupId");
        }

        // Check if message exists
        var message = await messageRepository.GetByIdAsync(request.MessageId, cancellationToken: cancellationToken);
        if (message == null)
        {
            throw new BadRequestException($"Message with ID {request.MessageId} not found");
        }

        // Check if message is already pinned
        Domain.Entities.PinMessage? existingPin = null;
        if (request.ConversationId != null)
        {
            existingPin = await repository.GetSingleAsync(
                pm => pm.MessageId == request.MessageId && pm.ConversationId == request.ConversationId.Value,
                cancellationToken: cancellationToken);
        }
        else if (request.GroupId != null)
        {
            existingPin = await repository.GetSingleAsync(
                pm => pm.MessageId == request.MessageId && pm.GroupId == request.GroupId.Value,
                cancellationToken: cancellationToken);
        }

        if (existingPin != null)
        {
            return AppResponse<PinMessageDto>.Error("Message is already pinned");
        }

        // Check pin limit (maximum 10 pins per conversation/group)
        int pinCount = 0;
        if (request.ConversationId != null)
        {
            pinCount = await repository.GetQuery()
                                        .AsNoTracking()
                                        .Where(pm => pm.ConversationId == request.ConversationId.Value)
                                        .CountAsync(cancellationToken);
        }
        else if (request.GroupId != null)
        {
            pinCount = await repository.GetQuery()
                                        .AsNoTracking()
                                        .Where(pm => pm.GroupId == request.GroupId.Value)
                                        .CountAsync(cancellationToken);
        }

        if (pinCount >= 10)
        {
            return AppResponse<PinMessageDto>.Error("Maximum of 10 pinned messages reached. Please unpin a message first.");
        }

        var pinnedByUser = await userManager.FindByIdAsync(request.PinnedByUserId.ToString());
        if (pinnedByUser == null)
        {
            throw new BadRequestException($"User with ID {request.PinnedByUserId} not found");
        }

        // Create pin message
        var pinMessage = new Domain.Entities.PinMessage
        {
            MessageId = request.MessageId,
            PinnedByUserId = request.PinnedByUserId,
            ConversationId = request.ConversationId,
            GroupId = request.GroupId,
            PinnedByUser = pinnedByUser
        };

        var notification = new Message
        {
            Content = $"A new message pinned by {pinnedByUser.GetFullName()}",
            MessageType = MessageTypes.Notification,
            ConversationId = request.ConversationId,
            GroupId = request.GroupId,
            SenderId = request.PinnedByUserId,
            SenderUserName = pinnedByUser?.UserName ?? ""
        };

        await repository.AddAsync(pinMessage, cancellationToken);
        await messageRepository.AddAsync(notification, cancellationToken);
        
        var pinMessageDto = new PinMessageDto
        {
            Id = pinMessage.Id,
            MessageId = pinMessage.MessageId,
            PinnedByUserId = pinMessage.PinnedByUserId,
            PinnedByUserName = pinMessage.PinnedByUser.GetFullName(),
            CreatedAt = pinMessage.CreatedAt,
            ConversationId = pinMessage.ConversationId,
            GroupId = pinMessage.GroupId,
            Message = message.Adapt<MessageDto>()
        };

        var onEventObject = new
        {
            MessageId = request.MessageId,
            ConversationId = request.ConversationId,
            GroupId = request.GroupId,
            Message = notification.Adapt<MessageDto>()
        };

        if (request.ConversationId.HasValue)
        {
            var conversation = await conversationRepository.GetByIdAsync(request.ConversationId.Value, cancellationToken: cancellationToken);
            if (conversation != null)
            {
                // Send to both users in the conversation
                await hubContext.Clients
                    .Users([request.PinnedByUserId.ToString(), conversation.GetOtherUserId(request.PinnedByUserId).ToString()])
                    .SendAsync("OnMessagePinned", onEventObject, cancellationToken);
            }
        }
        else if (request.GroupId.HasValue)
        {
            await hubContext.Clients.Group(request.GroupId.Value.ToString())
                .SendAsync("OnMessagePinned", onEventObject, cancellationToken);
        }   


        return AppResponse<PinMessageDto>.Success(pinMessageDto);
    }
}

