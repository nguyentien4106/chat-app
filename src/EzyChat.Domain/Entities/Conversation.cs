using EzyChat.Domain.Entities.Base;

namespace EzyChat.Domain.Entities;

// Domain/Entities/Conversation.cs
public class Conversation : Entity<Guid>
{
    public Guid SenderId { get; set; }
    public ApplicationUser Sender { get; set; } = null!;
    public Guid ReceiverId { get; set; }
    public ApplicationUser Receiver { get; set; } = null!;
    public ICollection<Message> Messages { get; set; } = [];
    public ICollection<PinMessage> PinnedMessages { get; set; } = [];
    public DateTime LastMessageAt { get; set; }
    
    // Helper method to check if a user is part of this conversation
    public bool HasUser(Guid userId)
    {
        return SenderId == userId || ReceiverId == userId;
    }
    
    // Helper method to get the other user in the conversation
    public Guid GetOtherUserId(Guid currentUserId)
    {
        return SenderId == currentUserId ? ReceiverId : SenderId;
    }
}