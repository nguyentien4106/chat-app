namespace EzyChat.Application.DTOs.Messages;

public class PinMessageDto
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    public Guid PinnedByUserId { get; set; }
    public string PinnedByUserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid? ConversationId { get; set; }
    public Guid? GroupId { get; set; }
    public MessageDto Message { get; set; } = null!;
}
