namespace ChatApp.Application.Commands.Messages.MarkRead;

public class MarkReadHandler(
    IRepository<Message> messageRepository,
    IRepository<Conversation> conversationRepository
    ) : ICommandHandler<MarkReadCommand, AppResponse<int>>
{
    public async Task<AppResponse<int>> Handle(MarkReadCommand request, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.GetByIdAsync(request.ConversationId);
        if (conversation == null)
        {
            return AppResponse<int>.Success(0);
        }
        
        var otherUserId = conversation.GetOtherUserId(request.CurrentUserId);
        
        // Get all unread messages in the conversation from the sender
        var messages = await messageRepository.GetAllAsync(
            filter: m => m.ConversationId == request.ConversationId 
                      && m.SenderId == otherUserId
                      && !m.IsRead,
            cancellationToken: cancellationToken
        );

        var messagesList = messages.ToList();
        if (messagesList.Count == 0)
        {
            return AppResponse<int>.Success(0);
        }

        // Mark all messages as read in memory
        foreach (var message in messagesList)
        {
            message.IsRead = true;
        }

        // Perform single bulk update to database
        await messageRepository.UpdateRangeAsync(messagesList, cancellationToken);

        return AppResponse<int>.Success(messagesList.Count);
    }
}
