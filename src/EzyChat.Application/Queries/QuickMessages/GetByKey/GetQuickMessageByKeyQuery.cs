using EzyChat.Application.DTOs.QuickMessages;
using EzyChat.Application.Models;

namespace EzyChat.Application.Queries.QuickMessages;

public class GetQuickMessageByKeyQuery : IQuery<AppResponse<List<QuickMessageDto>>>
{
    public string Key { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}
