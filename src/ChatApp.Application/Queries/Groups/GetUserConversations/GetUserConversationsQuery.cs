using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Models;

namespace ChatApp.Application.Queries.Groups.GetUserConversations;

public class GetUserConversationsQuery: IQuery<AppResponse<List<ConversationDto>>>
{
    public Guid UserId { get; set; }
}