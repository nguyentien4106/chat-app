using System.Text.Json;
using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Hubs;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using ChatApp.Domain.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ChatApp.Application.Commands.Messages.SendMessage;

public class SendMessageHandler(
    IRepository<Message> messageRepository,
    IHubContext<ChatHub> hubContext,
    ILogger<SendMessageHandler> logger)
    : ICommandHandler<SendMessageCommand, AppResponse<MessageDto>>
{
    public async Task<AppResponse<MessageDto>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var message = request.Adapt<Message>();
        await messageRepository.AddAsync(message, cancellationToken);

        // Load sender info
        logger.LogInformation("Loading sender info for user ID: {UserId}", JsonSerializer.Serialize(message));

        var messageDto = message.Adapt<MessageDto>();
        messageDto.SenderUsername = message.Sender?.UserName ?? "";

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