namespace EzyChat.Application.DTOs.QuickMessages;

public class UpdateQuickMessageDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
}
