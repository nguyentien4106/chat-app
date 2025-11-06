using EzyChat.Domain.Entities.Base;
using EzyChat.Domain.Enums;

namespace EzyChat.Domain.Entities;

public class Message : Entity<Guid>
{
    public string? Content { get; set; }
    public MessageTypes MessageType { get; set; } = MessageTypes.Text;
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public string? FileType { get; set; }
    
    public long? FileSize { get; set; }
    public Guid SenderId { get; set; }
    public ApplicationUser Sender { get; set; } = null!;
    public Guid? ConversationId { get; set; }
    public Conversation? Conversation { get; set; }
    public Guid? GroupId { get; set; }
    public Group? Group { get; set; }
    
    public bool IsRead { get; set; }
    
    // Helper method to check if message is a direct message
    public bool IsDirectMessage() => ConversationId.HasValue;
    
    // Helper method to check if message is a group message
    public bool IsGroupMessage() => GroupId.HasValue;
}