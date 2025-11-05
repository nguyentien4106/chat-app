using EzyChat.Application.DTOs.Common;
using EzyChat.Application.Models;

namespace EzyChat.Application.Queries.Conversations.GetMessagesByConversationId;

public class GetMessagesByConversationIdQuery : PaginationRequest, IQuery<AppResponse<PagedResult<MessageDto>>>
{
    public Guid ConversationId { get; set; }
    public Guid CurrentUserId { get; set; }
}
