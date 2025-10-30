using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.User1Id)
            .IsRequired();
        
        builder.Property(e => e.User2Id)
            .IsRequired();
        
        builder.Property(e => e.LastMessageAt)
            .IsRequired();
        
        // Create a unique index on User1Id and User2Id to prevent duplicate conversations
        // This ensures that there's only one conversation between any two users
        builder.HasIndex(e => new { e.User1Id, e.User2Id })
            .IsUnique()
            .HasDatabaseName("IX_Conversations_Users");
        
        // Create an index on LastMessageAt for efficient sorting of conversations
        builder.HasIndex(e => e.LastMessageAt)
            .HasDatabaseName("IX_Conversations_LastMessageAt");
    }
}
