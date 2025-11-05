using EzyChat.Application.DTOs.Common;
using EzyChat.Application.Hubs;
using EzyChat.Application.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace EzyChat.Application.Commands.Messages.SendMessage.Strategy;

public class GroupMessageStrategy(
    IRepository<Message> messageRepository,
    IHubContext<ChatHub> hubContext,
    ILogger<GroupMessageStrategy> logger
) : ISendMessageStrategy
{
    public bool CanHandle(SendMessageCommand command)
    {
        return command.GroupId.HasValue;
    }

    public async Task<AppResponse<MessageDto>> SendAsync(SendMessageCommand command, CancellationToken cancellationToken)
    {
        var message = command.Adapt<Message>();
        message.GroupId = command.GroupId!.Value;
        
        await messageRepository.AddAsync(message, cancellationToken);
        
        // Reload message with Group navigation property
        message = await messageRepository.GetByIdAsync(
            message.Id, 
            includeProperties: ["Group", "Sender"],
            cancellationToken: cancellationToken) ?? message;
        
        logger.LogInformation("Group message sent: {MessageId} from user: {SenderId} to group: {GroupId}", 
            message.Id, command.SenderId, command.GroupId);

        var messageDto = message.Adapt<MessageDto>();
        messageDto.GroupName = message.Group?.Name;
        
        // Send via SignalR to all group members
        await hubContext.Clients.Group(command.GroupId.Value.ToString())
            .SendAsync("ReceiveMessage", messageDto, cancellationToken);

        return AppResponse<MessageDto>.Success(messageDto);
    }
}