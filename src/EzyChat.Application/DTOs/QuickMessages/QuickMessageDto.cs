namespace EzyChat.Application.DTOs.QuickMessages;

public class QuickMessageDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
