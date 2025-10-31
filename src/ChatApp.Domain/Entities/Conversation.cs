using ChatApp.Domain.Entities.Base;

namespace ChatApp.Domain.Entities;

// Domain/Entities/Conversation.cs
public class Conversation : Entity<Guid>
{
    public Guid User1Id { get; set; }
    public Guid User2Id { get; set; }
    public ICollection<Message> Messages { get; set; } = [];
    public DateTime LastMessageAt { get; set; }
    
    // Helper method to check if a user is part of this conversation
    public bool HasUser(Guid userId)
    {
        return User1Id == userId || User2Id == userId;
    }
    
    // Helper method to get the other user in the conversation
    public Guid GetOtherUserId(Guid currentUserId)
    {
        return User1Id == currentUserId ? User2Id : User1Id;
    }
}