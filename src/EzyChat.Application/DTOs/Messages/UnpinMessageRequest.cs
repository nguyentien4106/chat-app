namespace EzyChat.Application.DTOs.Messages;
public class UnpinMessageRequest
{
    public Guid MessageId { get; set; }
    public Guid? ConversationId { get; set; }
    public Guid? GroupId { get; set; }
    public Guid UnpinnedByUserId { get; set; }
}