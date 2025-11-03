using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Models;

namespace ChatApp.Application.Commands.Messages.SendMessage.Strategy;

public interface ISendMessageStrategy
{
    Task<AppResponse<MessageDto>> SendAsync(SendMessageCommand command, CancellationToken cancellationToken);
    bool CanHandle(SendMessageCommand command);
}