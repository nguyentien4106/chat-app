using ChatApp.Application.DTOs.Common;
<<<<<<< HEAD

namespace ChatApp.Application.Queries.Messages.GetConversationMessage;

public class GetConversationMessagesQuery : IQuery<List<MessageDto>>
=======
using ChatApp.Application.Models;

namespace ChatApp.Application.Queries.Messages.GetConversationMessages;

public class GetConversationMessagesQuery : IQuery<AppResponse<List<MessageDto>>>
>>>>>>> a957673 (initial)
{
    public Guid User1Id { get; set; }
    public Guid User2Id { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 50;
}