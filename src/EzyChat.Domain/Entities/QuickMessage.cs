using EzyChat.Domain.Entities.Base;

namespace EzyChat.Domain.Entities;

public class QuickMessage : Entity<Guid>
{
    public string Content { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;
}