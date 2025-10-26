using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Hubs;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Application.Commands.Messages.SendMessage;

public class SendMessageHandler(IChatAppDbContext context, IHubContext<ChatHub> hubContext)
    : ICommandHandler<SendMessageCommand, AppResponse<MessageDto>>
{
    public async Task<AppResponse<MessageDto>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var message = request.Adapt<Message>();
        context.Messages.Add(message);
        await context.SaveChangesAsync(cancellationToken);

        // Load sender info
        var sender = await context.Users.FindAsync(message.SenderId, cancellationToken);

        var messageDto = message.Adapt<MessageDto>();
        messageDto.SenderUsername = sender?.UserName ?? "";

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
}