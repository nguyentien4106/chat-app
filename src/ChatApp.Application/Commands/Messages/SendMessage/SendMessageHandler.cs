<<<<<<< HEAD
namespace ChatApp.Application.Commands.Messages.SendMessage;

public class SendMessageHandler
{
    
=======
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
        var message = new Message
        {
            Id = Guid.NewGuid(),
            Content = request.Content,
            Type = request.Type,
            FileUrl = request.FileUrl,
            FileName = request.FileName,
            FileType = request.FileType,
            FileSize = request.FileSize,
            SenderId = request.SenderId,
            ReceiverId = request.ReceiverId,
            GroupId = request.GroupId,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        context.Messages.Add(message);
        await context.SaveChangesAsync(cancellationToken);

        // Load sender info
        var sender = await context.Users.FindAsync(message.SenderId, cancellationToken);

        var messageDto = new MessageDto
        {
            Id = message.Id,
            Content = message.Content,
            Type = message.Type,
            FileUrl = message.FileUrl,
            FileName = message.FileName,
            FileType = message.FileType,
            FileSize = message.FileSize,
            SenderId = message.SenderId,
            SenderUsername = sender?.UserName,
            ReceiverId = message.ReceiverId,
            GroupId = message.GroupId,
            CreatedAt = message.CreatedAt,
            IsRead = message.IsRead
        };

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
            await hubContext.Clients.User(request.SenderId.ToString())
                .SendAsync("ReceiveMessage", messageDto, cancellationToken);
        }

        return AppResponse<MessageDto>.Success(messageDto);
    }
>>>>>>> a957673 (initial)
}