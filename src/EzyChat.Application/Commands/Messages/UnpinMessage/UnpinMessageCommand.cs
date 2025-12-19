using EzyChat.Application.Models;

namespace EzyChat.Application.Commands.Messages.UnpinMessage;

public class UnpinMessageCommand : ICommand<AppResponse<bool>>
{
    public Guid MessageId { get; set; }
    public Guid UnpinnedByUserId { get; set; }
    public Guid? ConversationId { get; set; }
    public Guid? GroupId { get; set; }
}
