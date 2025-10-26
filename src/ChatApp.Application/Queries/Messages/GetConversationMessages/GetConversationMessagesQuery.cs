using ChatApp.Application.DTOs.Common;

namespace ChatApp.Application.Queries.Messages.GetConversationMessage;

public class GetConversationMessagesQuery : IQuery<List<MessageDto>>
{
    public Guid User1Id { get; set; }
    public Guid User2Id { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 50;
}