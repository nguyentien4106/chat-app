using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Models;

namespace ChatApp.Application.Commands.Messages.SendMessage.Strategy;

public class SendMessageStrategyContext(IEnumerable<ISendMessageStrategy> strategies)
{
    private readonly IEnumerable<ISendMessageStrategy> _strategies = strategies;

    public async Task<AppResponse<MessageDto>> ExecuteAsync(SendMessageCommand command, CancellationToken cancellationToken)
    {
        var strategy = _strategies.FirstOrDefault(s => s.CanHandle(command));
        
        if (strategy == null)
        {
            throw new InvalidOperationException("No suitable strategy found for the given message command.");
        }

        return await strategy.SendAsync(command, cancellationToken);
    }
}