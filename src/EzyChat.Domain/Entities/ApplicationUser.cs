using EzyChat.Domain.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace EzyChat.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public UserRefreshToken? RefreshToken { get; set; } = null!;
    public DateTime Created { get; set; }
    public string? CreatedBy { get; set; }

    // Navigation Properties
    public ICollection<GroupMember> GroupMembers { get; set; } = [];
    
    // Conversations where this user is User1
    public ICollection<Conversation> ConversationsAsSender { get; set; } = [];
    
    // Conversations where this user is User2
    public ICollection<Conversation> ConversationsAsReceiver { get; set; } = [];
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    
    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }
}
