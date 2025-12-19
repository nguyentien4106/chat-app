using EzyChat.Application.DTOs.QuickMessages;
using EzyChat.Application.Models;

namespace EzyChat.Application.Queries.QuickMessages;

public class GetUserQuickMessagesQuery : IQuery<AppResponse<List<QuickMessageDto>>>
{
    public Guid UserId { get; set; }
}
