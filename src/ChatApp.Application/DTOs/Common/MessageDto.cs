using ChatApp.Domain.Enums;

namespace ChatApp.Application.DTOs.Common;

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
    public string? SenderUsername { get; set; }
    
    public Guid ReceiverId { get; set; }
    public Guid? ConversationId { get; set; }
    public Guid? GroupId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}