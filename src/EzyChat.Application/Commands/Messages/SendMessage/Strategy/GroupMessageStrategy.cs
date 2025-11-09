using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace EzyChat.Application.Commands.Messages.SendMessage.Strategy;

public class GroupMessageStrategy(
    IRepository<Message> messageRepository,
    IHubContext<ChatHub> hubContext,
    IUserRepository userRepository
) : ISendMessageStrategy
{
    public bool CanHandle(SendMessageCommand command)
    {
        return command is { GroupId: not null, Type: "group" };
    }

    public async Task<AppResponse<MessageDto>> SendAsync(SendMessageCommand command, CancellationToken cancellationToken)
    {
        var message = command.Adapt<Message>();
        message.GroupId = command.GroupId!.Value;
        
        await messageRepository.AddAsync(message, cancellationToken);

        var messageDto = message.Adapt<MessageDto>();
        messageDto.SenderUserName = command.SenderUserName;
        messageDto.GroupName = command.GroupName;
        
        // Send via SignalR to all group members
        await hubContext.Clients.Group(command.GroupId.Value.ToString())
            .SendAsync("OnReceiveMessage", messageDto, cancellationToken);

        return AppResponse<MessageDto>.Success(messageDto);
    }
}