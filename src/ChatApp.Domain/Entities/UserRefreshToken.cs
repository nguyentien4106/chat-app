using ChatApp.Domain.Entities.Base;

namespace ChatApp.Domain.Entities;

public class UserRefreshToken : Entity<Guid>
{
    public Guid ApplicationUserId { get; set; }

    public string RefreshToken { get; set; } = string.Empty;

    public ApplicationUser ApplicationUser {get;set;} = null!;
}