using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.SenderId)
            .IsRequired();
        
        builder.Property(e => e.ReceiverId)
            .IsRequired();
        
        builder.Property(e => e.LastMessageAt)
            .IsRequired();
        
        // Configure navigation properties with foreign keys
        builder.HasOne(e => e.Sender)
            .WithMany(u => u.ConversationsAsSender)
            .HasForeignKey(e => e.SenderId)
            .OnDelete(DeleteBehavior.Restrict) // Prevent accidental deletion
            .IsRequired();
        
        builder.HasOne(e => e.Receiver)
            .WithMany(u => u.ConversationsAsReceiver)
            .HasForeignKey(e => e.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict) // Prevent accidental deletion
            .IsRequired();
        
        // Create a unique index on User1Id and User2Id to prevent duplicate conversations
        // This ensures that there's only one conversation between any two users
        builder.HasIndex(e => new { User1Id = e.SenderId, User2Id = e.ReceiverId })
            .IsUnique()
            .HasDatabaseName("IX_Conversations_Users");
        
        // Create an index on LastMessageAt for efficient sorting of conversations
        builder.HasIndex(e => e.LastMessageAt)
            .HasDatabaseName("IX_Conversations_LastMessageAt");
    }
}
