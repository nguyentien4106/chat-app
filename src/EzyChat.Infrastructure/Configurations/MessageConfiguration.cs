using EzyChat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EzyChat.Infrastructure.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Content)
            .HasMaxLength(4000);
        
        builder.Property(e => e.MessageType)
            .IsRequired();
        
        builder.Property(e => e.FileUrl)
            .HasMaxLength(500);
        
        builder.Property(e => e.FileName)
            .HasMaxLength(255);
        
        builder.Property(e => e.FileType)
            .HasMaxLength(100);
        
        // Sender relationship
        builder.HasOne(e => e.Sender)
            .WithMany()
            .HasForeignKey(e => e.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Conversation relationship (for direct messages)
        builder.HasOne(e => e.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(e => e.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Group relationship (for group messages)
        builder.HasOne(e => e.Group)
            .WithMany(g => g.Messages)
            .HasForeignKey(e => e.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indexes for efficient querying
        builder.HasIndex(e => e.ConversationId);
        builder.HasIndex(e => e.GroupId);
        builder.HasIndex(e => e.SenderId);
        builder.HasIndex(e => e.CreatedAt);
        
        // Add a check constraint to ensure a message belongs to either a Conversation OR a Group, not both
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_Message_ConversationOrGroup",
            "(\"ConversationId\" IS NOT NULL AND \"GroupId\" IS NULL) OR (\"ConversationId\" IS NULL AND \"GroupId\" IS NOT NULL)"
        ));
    }
}
