namespace EzyChat.Application.DTOs.Common;

public class ConversationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string LastMessage { get; set; } = string.Empty;
    public DateTime LastMessageAt { get; set; }
    public int UnreadCount { get; set; }
    
    public bool IsLastMessageMine { get; set; }
    
    public string UserFullName { get; set; }
}