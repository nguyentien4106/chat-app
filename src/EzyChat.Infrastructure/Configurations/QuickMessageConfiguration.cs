using EzyChat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EzyChat.Infrastructure.Configurations;

public class QuickMessageConfiguration : IEntityTypeConfiguration<QuickMessage>
{
    public void Configure(EntityTypeBuilder<QuickMessage> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Content)
            .IsRequired()
            .HasMaxLength(10000);
        
        builder.Property(e => e.Key)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .IsRequired();
        
        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone");
        
        // User relationship
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Create unique index on Key + UserId to prevent duplicate keys per user
        builder.HasIndex(e => new { e.Key, e.UserId })
            .IsUnique();
        
        // Index for faster lookups
        builder.HasIndex(e => e.UserId);
    }
}
