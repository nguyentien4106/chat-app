using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Models;

namespace ChatApp.Application.Queries.Conversations.GetMessagesByConversationId;

public class GetMessagesByConversationIdQuery : PaginationRequest, IQuery<AppResponse<PagedResult<MessageDto>>>
{
    public Guid ConversationId { get; set; }
    public Guid CurrentUserId { get; set; }
}
