using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Exceptions;
using EzyChat.Application.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace EzyChat.Application.Commands.Messages.UnpinMessage;

public class UnpinMessageHandler(
    IRepository<Domain.Entities.PinMessage> repository,
    IRepository<Conversation> conversationRepository,
    IHubContext<ChatHub> hubContext,
    UserManager<ApplicationUser> userManager,
    IRepository<Message> messageRepository
    ) : ICommandHandler<UnpinMessageCommand, AppResponse<bool>>
{
    public async Task<AppResponse<bool>> Handle(UnpinMessageCommand request, CancellationToken cancellationToken)
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

        // Find the pinned message
        Domain.Entities.PinMessage? pinMessage = null;
        if (request.ConversationId != null)
        {
            pinMessage = await repository.GetSingleAsync(
                pm => pm.MessageId == request.MessageId && pm.ConversationId == request.ConversationId.Value,
                cancellationToken: cancellationToken);
        }
        else if (request.GroupId != null)
        {
            pinMessage = await repository.GetSingleAsync(
                pm => pm.MessageId == request.MessageId && pm.GroupId == request.GroupId.Value,
                cancellationToken: cancellationToken);
        }

        if (pinMessage == null)
        {
            throw new BadRequestException("Pinned message not found");
        }

        var pinnedByUser = await userManager.FindByIdAsync(request.UnpinnedByUserId.ToString());
        if (pinnedByUser == null)
        {
            throw new BadRequestException($"User with ID {request.UnpinnedByUserId} not found");
        }

        var notification = new Message
        {
            Content = $"A message unpinned by {pinnedByUser.GetFullName()}",
            MessageType = Domain.Enums.MessageTypes.Notification,
            ConversationId = request.ConversationId,
            GroupId = request.GroupId,
            SenderId = request.UnpinnedByUserId,
            SenderUserName = pinnedByUser?.UserName ?? ""
        };
        // Remove the pin
        await repository.DeleteAsync(pinMessage, cancellationToken);
        await messageRepository.AddAsync(notification, cancellationToken);

        var onEventObject = new
        {
            MessageId = request.MessageId,
            ConversationId = request.ConversationId,
            GroupId = request.GroupId,
            Message = notification.Adapt<MessageDto>()
        };

        // Notify via SignalR
        if (request.ConversationId.HasValue)
        {
            var conversation = await conversationRepository.GetByIdAsync(request.ConversationId.Value, cancellationToken: cancellationToken);
            if (conversation != null)
            {
                await hubContext.Clients
                    .Users([request.UnpinnedByUserId.ToString(), conversation.GetOtherUserId(request.UnpinnedByUserId).ToString()])
                    .SendAsync("OnMessageUnpinned", onEventObject, cancellationToken);
            }
        }
        else if (request.GroupId.HasValue)
        {
            await hubContext.Clients.Group(request.GroupId.Value.ToString())
                .SendAsync("OnMessageUnpinned", onEventObject, cancellationToken);
        }

        return AppResponse<bool>.Success(true);
    }
}
