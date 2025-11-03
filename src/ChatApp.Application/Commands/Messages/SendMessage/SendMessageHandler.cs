using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Models;
using ChatApp.Application.Commands.Messages.SendMessage.Strategy;

namespace ChatApp.Application.Commands.Messages.SendMessage;

public class SendMessageHandler(SendMessageStrategyContext strategyContext) 
    : ICommandHandler<SendMessageCommand, AppResponse<MessageDto>>
{
    public async Task<AppResponse<MessageDto>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        return await strategyContext.ExecuteAsync(request, cancellationToken);
    }
}