using EzyChat.Application.Interfaces;
using EzyChat.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EzyChat.Infrastructure;

public class EzyChatDbContext(DbContextOptions<EzyChatDbContext> options) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options), IEzyChatDbContext
{
    public DbSet<UserRefreshToken> UserRefreshTokens => Set<UserRefreshToken>();
    
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<GroupMember> GroupMembers => Set<GroupMember>();
    public DbSet<Message> Messages => Set<Message>();
    
    public DbSet<Conversation> Conversations => Set<Conversation>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Enable PostgreSQL extensions
        modelBuilder.HasPostgresExtension("pg_trgm");
        
        // Apply all entity configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

}
