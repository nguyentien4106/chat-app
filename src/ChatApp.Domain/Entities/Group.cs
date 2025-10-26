namespace ChatApp.Domain.Entities;

// Domain/Entities/Group.cs
public class Group
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CreatedById { get; set; }
    public ApplicationUser CreatedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string? InviteCode { get; set; }
    public DateTime? InviteCodeExpiresAt { get; set; }
    public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}