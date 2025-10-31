namespace ChatApp.Application.DTOs.Common;

public class ConversationDto
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string LastMessage { get; set; } = string.Empty;
    public DateTime LastMessageAt { get; set; }
    public int UnreadCount { get; set; }
}