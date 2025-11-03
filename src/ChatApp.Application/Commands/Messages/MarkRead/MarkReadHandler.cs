using ChatApp.Application.Models;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories;

namespace ChatApp.Application.Commands.Messages.MarkRead;

public class MarkReadHandler(
    IRepository<Message> messageRepository
) : ICommandHandler<MarkReadCommand, AppResponse<int>>
{
    public async Task<AppResponse<int>> Handle(MarkReadCommand request, CancellationToken cancellationToken)
    {
        // Get all unread messages in the conversation from the sender
        var messages = await messageRepository.GetAllAsync(
            filter: m => m.ConversationId == request.ConversationId 
                      && m.SenderId == request.SenderId 
                      && !m.IsRead,
            cancellationToken: cancellationToken
        );

        if (!messages.Any())
        {
            return AppResponse<int>.Success(0);
        }

        // Mark all messages as read
        var count = 0;
        foreach (var message in messages)
        {
            message.IsRead = true;
            await messageRepository.UpdateAsync(message, cancellationToken);
            count++;
        }

        return AppResponse<int>.Success(count);
    }
}
