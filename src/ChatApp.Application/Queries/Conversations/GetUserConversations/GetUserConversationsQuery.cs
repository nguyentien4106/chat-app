using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Models;

namespace ChatApp.Application.Queries.Conversations.GetUserConversations;

public class GetUserConversationsQuery : PaginationRequest, IQuery<AppResponse<PagedResult<ConversationDto>>>
{
    public Guid UserId { get; set; }
}
