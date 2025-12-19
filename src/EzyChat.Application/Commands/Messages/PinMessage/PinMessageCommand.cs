using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Models;

namespace EzyChat.Application.Commands.Messages.PinMessage;

public class PinMessageCommand : ICommand<AppResponse<PinMessageDto>>
{
    public Guid MessageId { get; set; }
    public Guid? ConversationId { get; set; }
    public Guid? GroupId { get; set; }
    public Guid PinnedByUserId { get; set; }
}
