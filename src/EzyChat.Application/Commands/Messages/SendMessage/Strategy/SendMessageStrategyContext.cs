using EzyChat.Application.DTOs.Common;
using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Models;

namespace EzyChat.Application.Commands.Messages.SendMessage.Strategy;

public class SendMessageStrategyContext(IEnumerable<ISendMessageStrategy> strategies)
{
    public async Task<AppResponse<MessageDto>> ExecuteAsync(SendMessageCommand command, CancellationToken cancellationToken)
    {
        var strategy = strategies.FirstOrDefault(s => s.CanHandle(command));
        
        if (strategy == null)
        {
            throw new InvalidOperationException("No suitable strategy found for the given message command.");
        }

        return await strategy.SendAsync(command, cancellationToken);
    }
}