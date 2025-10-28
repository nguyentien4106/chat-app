using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Configurations;

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
        
        builder.HasOne(e => e.Sender)
            .WithMany()
            .HasForeignKey(e => e.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(e => e.Receiver)
            .WithMany()
            .HasForeignKey(e => e.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(e => e.Group)
            .WithMany(g => g.Messages)
            .HasForeignKey(e => e.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(e => new { e.SenderId, e.ReceiverId });
    }
}
