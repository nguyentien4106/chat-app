using EzyChat.Domain.Enums;

namespace EzyChat.Application.DTOs.Messages;

public class MessageDto
{
    public Guid Id { get; set; }
    public string? Content { get; set; }
    public MessageTypes MessageType { get; set; }
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
    public Guid SenderId { get; set; }
    public string? SenderUserName { get; set; }
    public Guid ReceiverId { get; set; }
    
    public Guid? ConversationId { get; set; }
    public Guid? GroupId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public bool IsRead { get; set; }

    public string? GroupName { get; set; } = string.Empty;
    
    public bool IsNewConversation { get; set; }

    public string SenderFullName { get; set; } = string.Empty; 
}