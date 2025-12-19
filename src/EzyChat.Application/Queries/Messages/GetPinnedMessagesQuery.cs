using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Models;

namespace EzyChat.Application.Queries.Messages;

public class GetPinnedMessagesQuery : IQuery<AppResponse<List<PinMessageDto>>>
{
    public Guid? ConversationId { get; set; }
    public Guid? GroupId { get; set; }
}
