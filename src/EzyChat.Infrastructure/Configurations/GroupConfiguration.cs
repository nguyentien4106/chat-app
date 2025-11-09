using EzyChat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EzyChat.Infrastructure.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(e => e.Description)
            .HasMaxLength(500);
        
        // Configure datetime columns as timestamp without timezone
        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone");
        
        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone");
        
        builder.Property(e => e.InviteCodeExpiresAt)
            .HasColumnType("timestamp without time zone");
        
        builder.HasMany(e => e.Messages)
            .WithOne(e => e.Group)
            .HasForeignKey(e => e.GroupId);

        builder.HasMany(e => e.Members)
            .WithOne(e => e.Group)
            .HasForeignKey(e => e.GroupId);

        builder.HasIndex(e => e.InviteCode);
    }
}
