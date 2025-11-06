using EzyChat.Application.DTOs.Common;
using EzyChat.Application.Models;
using EzyChat.Application.Commands.Messages.SendMessage.Strategy;
using EzyChat.Application.DTOs.Messages;

namespace EzyChat.Application.Commands.Messages.SendMessage;

public class SendMessageHandler(SendMessageStrategyContext strategyContext) 
    : ICommandHandler<SendMessageCommand, AppResponse<MessageDto>>
{
    public async Task<AppResponse<MessageDto>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        return await strategyContext.ExecuteAsync(request, cancellationToken);
    }
}