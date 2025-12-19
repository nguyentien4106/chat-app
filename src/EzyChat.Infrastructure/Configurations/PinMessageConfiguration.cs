using EzyChat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EzyChat.Infrastructure.Configurations;

public class PinMessageConfiguration : IEntityTypeConfiguration<PinMessage>
{
    public void Configure(EntityTypeBuilder<PinMessage> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .IsRequired();
        
        // Message relationship
        builder.HasOne(e => e.Message)
            .WithMany(m => m.PinMessages)
            .HasForeignKey(e => e.MessageId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // PinnedByUser relationship
        builder.HasOne(e => e.PinnedByUser)
            .WithMany()
            .HasForeignKey(e => e.PinnedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Conversation relationship (optional)
        builder.HasOne(e => e.Conversation)
            .WithMany(c => c.PinnedMessages)
            .HasForeignKey(e => e.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Group relationship (optional)
        builder.HasOne(e => e.Group)
            .WithMany(g => g.PinnedMessages)
            .HasForeignKey(e => e.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Ensure a message can only be pinned once per conversation/group
        builder.HasIndex(e => new { e.MessageId, e.ConversationId })
            .IsUnique();
        
        builder.HasIndex(e => new { e.MessageId, e.GroupId })
            .IsUnique();
        
        // Index for efficient queries
        builder.HasIndex(e => e.ConversationId);
        builder.HasIndex(e => e.GroupId);
    }
}
