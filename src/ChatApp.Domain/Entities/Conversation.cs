using ChatApp.Domain.Entities.Base;

namespace ChatApp.Domain.Entities;

// Domain/Entities/Conversation.cs
public class Conversation : Entity<Guid>
{
    public Guid User1Id { get; set; }
    public Guid User2Id { get; set; }
    public DateTime LastMessageAt { get; set; }
}