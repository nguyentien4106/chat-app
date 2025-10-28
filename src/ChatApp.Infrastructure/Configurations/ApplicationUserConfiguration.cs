﻿using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);
        
        // Create indexes for search optimization
        builder.HasIndex(u => u.UserName);
        builder.HasIndex(u => u.Email);
        
        // For PostgreSQL, you can add GIN index for full-text search
        // This requires the pg_trgm extension
        builder.HasIndex(u => u.UserName)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");
        
        builder.HasIndex(u => u.Email)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");
    }
}
