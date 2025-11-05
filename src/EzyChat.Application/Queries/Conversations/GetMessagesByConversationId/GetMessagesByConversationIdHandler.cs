using EzyChat.Application.DTOs.Common;
using EzyChat.Application.Interfaces;
using EzyChat.Application.Models;

namespace EzyChat.Application.Queries.Conversations.GetMessagesByConversationId;

public class GetMessagesByConversationIdHandler(
    IRepositoryPagedQuery<Message> messageRepository,
    IRepository<Conversation> conversationRepository
) : IQueryHandler<GetMessagesByConversationIdQuery, AppResponse<PagedResult<MessageDto>>>
{
    public async Task<AppResponse<PagedResult<MessageDto>>> Handle(
        GetMessagesByConversationIdQuery request, 
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
            request,
            filter: m => m.ConversationId == request.ConversationId,
            includeProperties: ["Sender"],
            cancellationToken: cancellationToken);

        // Map to DTOs
        var messageDtos = pagedMessages.Items.Adapt<List<MessageDto>>();
        
        var result = new PagedResult<MessageDto>(
            messageDtos,
            pagedMessages.TotalCount,
            pagedMessages.PageNumber,
            pagedMessages.PageSize);

        return AppResponse<PagedResult<MessageDto>>.Success(result);
    }
}
