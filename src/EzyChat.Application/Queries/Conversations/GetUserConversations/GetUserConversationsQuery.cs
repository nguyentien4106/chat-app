using EzyChat.Application.DTOs.Common;
using EzyChat.Application.Models;

namespace EzyChat.Application.Queries.Conversations.GetUserConversations;

public class GetUserConversationsQuery : PaginationRequest, IQuery<AppResponse<PagedResult<ConversationDto>>>
{
    public Guid UserId { get; set; }
}
