using EzyChat.Application.DTOs.Common;
using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Interfaces;

namespace EzyChat.Application.Queries.Conversations.GetConversationMessages;

public class GetConversationMessagesHandler(
    IMessageRepository messageRepository,
    IRepository<Conversation> conversationRepository
) : IQueryHandler<GetConversationMessagesQuery, AppResponse<PagedResult<MessageDto>>>
{
    public async Task<AppResponse<PagedResult<MessageDto>>> Handle(
        GetConversationMessagesQuery request, 
        CancellationToken cancellationToken)
    {
        // Verify the conversation exists and the user has access to it
        var conversation = await conversationRepository.GetByIdAsync(
            request.ConversationId,
            cancellationToken: cancellationToken);

        if (conversation == null)
        {
            return AppResponse<PagedResult<MessageDto>>.Fail("Conversation not found");
        }

        // Verify the current user is part of this conversation
        if (!conversation.HasUser(request.CurrentUserId))
        {
            return AppResponse<PagedResult<MessageDto>>.Fail("Access denied");
        }

        // Get paginated messages for this conversation
        var pagedMessages = await messageRepository.GetPagedResultAsync(
            request.BeforeDateTime,
            id: request.ConversationId,
            type: "conversation",
            includeProperties: ["Sender"],
            cancellationToken: cancellationToken);

        return AppResponse<PagedResult<MessageDto>>.Success(pagedMessages);
    }
}
