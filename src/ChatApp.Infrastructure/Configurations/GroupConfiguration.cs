using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Configurations;

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
        
        builder.HasMany(e => e.Messages)
            .WithOne(e => e.Group)
            .HasForeignKey(e => e.GroupId);

        builder.HasMany(e => e.Members)
            .WithOne(e => e.Group)
            .HasForeignKey(e => e.GroupId);

        builder.HasIndex(e => e.InviteCode);
    }
}
