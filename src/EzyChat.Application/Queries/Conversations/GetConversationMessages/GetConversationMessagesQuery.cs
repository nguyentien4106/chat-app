using EzyChat.Application.DTOs.Messages;

namespace EzyChat.Application.Queries.Conversations.GetConversationMessages;

public class GetConversationMessagesQuery : IQuery<AppResponse<PagedResult<MessageDto>>>
{
    public Guid ConversationId { get; set; }
    public Guid CurrentUserId { get; set; }
    public DateTime BeforeDateTime { get; set; }
}
