<<<<<<< HEAD
namespace ChatApp.Domain.Entities;

public class Message
{
    
=======
using ChatApp.Domain.Entities.Base;
using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Entities;

public class Message : Entity<Guid>
{
    public string? Content { get; set; }
    public MessageTypes Type { get; set; } = MessageTypes.Text;
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
    public Guid SenderId { get; set; }
    public ApplicationUser Sender { get; set; } = null!;
    public Guid? GroupId { get; set; }
    public Group? Group { get; set; }
    public Guid? ReceiverId { get; set; }
    public ApplicationUser? Receiver { get; set; }
    public bool IsRead { get; set; }
>>>>>>> a957673 (initial)
}