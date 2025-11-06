namespace EzyChat.Application.DTOs.Common;

public class GroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public int MemberCount { get; set; }

    public List<GroupMemberDto>? Members { get; set; }
}