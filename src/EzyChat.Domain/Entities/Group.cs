using EzyChat.Domain.Entities.Base;

namespace EzyChat.Domain.Entities;

// Domain/Entities/Group.cs
public class Group : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CreatedById { get; set; }
    public string? InviteCode { get; set; }
    public DateTime? InviteCodeExpiresAt { get; set; }
    
    public int MemberCount { get; set; }
    public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<PinMessage> PinnedMessages { get; set; } = new List<PinMessage>();
}