using EzyChat.Application.Models;

namespace EzyChat.Application.Commands.Messages.MarkRead;

public class MarkReadCommand : ICommand<AppResponse<int>>
{
    public Guid ConversationId { get; set; }
    public Guid CurrentUserId { get; set; }
}
