using ChatApp.Application.Models;

namespace ChatApp.Application.Commands.Messages.MarkRead;

public class MarkReadCommand : ICommand<AppResponse<int>>
{
    public Guid ConversationId { get; set; }
    public Guid CurrentUserId { get; set; }
}
