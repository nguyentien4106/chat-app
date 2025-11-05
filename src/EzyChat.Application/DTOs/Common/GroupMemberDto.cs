namespace EzyChat.Application.DTOs.Common;

public class GroupMemberDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
    public bool IsAdmin { get; set; }
}