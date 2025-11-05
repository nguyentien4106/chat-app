using EzyChat.Application.DTOs.Common;
using EzyChat.Application.Models;

namespace EzyChat.Application.Queries.Messages.GetGroupMessages;

public class GetGroupMessagesQuery : IQuery<AppResponse<List<MessageDto>>>
{
    public Guid GroupId { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 50;
}