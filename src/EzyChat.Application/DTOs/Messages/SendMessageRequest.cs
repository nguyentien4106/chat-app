using EzyChat.Domain.Enums;

namespace EzyChat.Application.DTOs.Messages;

public class SendMessageRequest
{
    public Guid SenderId { get; set; }
    
    public Guid? ReceiverId { get; set; }
    public Guid? ConversationId { get; set; }
    public Guid? GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string? Content { get; set; } = string.Empty;
    
    public MessageTypes MessageType { get; set; }
    
    public string? FileUrl { get; set; }
    
    public string? FileName { get; set; }
    
    public string? FileType { get; set; }
    
    public long FileSize { get; set; }
    
    public string Type { get; set; }
    
    public string SenderUserName { get; set; }
}