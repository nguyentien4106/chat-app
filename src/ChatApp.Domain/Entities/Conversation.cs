namespace ChatApp.Domain.Entities;

<<<<<<< HEAD
public class Conversation
{
    
=======
// Domain/Entities/Conversation.cs
public class Conversation
{
    public Guid Id { get; set; }
    public Guid User1Id { get; set; }
    public Guid User2Id { get; set; }
    public DateTime LastMessageAt { get; set; }
>>>>>>> a957673 (initial)
}