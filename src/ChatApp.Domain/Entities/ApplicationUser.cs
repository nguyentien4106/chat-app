using ChatApp.Domain.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>, IEntity<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public UserRefreshToken? RefreshToken { get; set; } = null!;
    public DateTime Created { get; set; }
    public string? CreatedBy { get; set; }

    // Navigation Properties
    public ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
