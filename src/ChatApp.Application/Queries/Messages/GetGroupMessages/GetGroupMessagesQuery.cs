<<<<<<< HEAD
namespace ChatApp.Application.Queries.Messages.GetGroupMessages;

public class GetGroupMessageQuery
{
    
=======
using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Models;

namespace ChatApp.Application.Queries.Messages.GetGroupMessages;

public class GetGroupMessagesQuery : IQuery<AppResponse<List<MessageDto>>>
{
    public Guid GroupId { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 50;
>>>>>>> a957673 (initial)
}