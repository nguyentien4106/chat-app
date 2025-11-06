using EzyChat.Domain.Enums;

namespace EzyChat.Application.DTOs.Messages;

public class SendFileMessageRequest
{
    public Guid? ConversationId { get; set; }
    public Guid? GroupId { get; set; }
    public string? Content { get; set; }
    public MessageTypes Type { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
}
