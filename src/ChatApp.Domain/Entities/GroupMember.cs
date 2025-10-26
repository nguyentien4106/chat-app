namespace ChatApp.Domain.Entities;

public class GroupMember
{
<<<<<<< HEAD
    
=======
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public DateTime JoinedAt { get; set; }
    public bool IsAdmin { get; set; }
>>>>>>> a957673 (initial)
}