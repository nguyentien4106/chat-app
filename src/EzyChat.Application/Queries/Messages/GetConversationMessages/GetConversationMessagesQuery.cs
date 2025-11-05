using EzyChat.Application.DTOs.Common;
using EzyChat.Application.Models;

namespace EzyChat.Application.Queries.Messages.GetConversationMessages;

public class GetConversationMessagesQuery : IQuery<AppResponse<List<MessageDto>>>
{
    public Guid User1Id { get; set; }
    public Guid User2Id { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 50;
}