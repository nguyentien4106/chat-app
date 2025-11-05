using EzyChat.Application.DTOs.Common;
using EzyChat.Application.Models;

namespace EzyChat.Application.Commands.Messages.SendMessage.Strategy;

public interface ISendMessageStrategy
{
    Task<AppResponse<MessageDto>> SendAsync(SendMessageCommand command, CancellationToken cancellationToken);
    bool CanHandle(SendMessageCommand command);
}