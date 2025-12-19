namespace EzyChat.Application.DTOs.Messages;

public class PinMessageRequest
{
    public Guid MessageId { get; set; }
    public Guid? ConversationId { get; set; }
    public Guid? GroupId { get; set; }
}
