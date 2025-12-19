using EzyChat.Domain.Entities.Base;

namespace EzyChat.Domain.Entities
{
    public class PinMessage : Entity<Guid>
    {
        public Guid MessageId { get; set; }
        public Guid PinnedByUserId { get; set; }
        
        // Either ConversationId or GroupId will be set, not both
        public Guid? ConversationId { get; set; }
        public Guid? GroupId { get; set; }

        // Navigation properties
        public Message Message { get; set; } = null!;
        public ApplicationUser PinnedByUser { get; set; } = null!;
        public Conversation? Conversation { get; set; }
        public Group? Group { get; set; }
    }
}