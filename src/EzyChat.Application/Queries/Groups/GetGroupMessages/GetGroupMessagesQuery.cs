using EzyChat.Application.DTOs.Messages;

namespace EzyChat.Application.Queries.Groups.GetGroupMessages;

public class GetGroupMessagesQuery : IQuery<AppResponse<PagedResult<MessageDto>>>
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }

    public DateTime BeforeDateTime { get; set; }
}
