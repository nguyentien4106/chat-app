using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Interfaces;

public interface IChatAppDbContext
{
    DbSet<ApplicationUser> Users { get; }
    DbSet<UserRefreshToken> UserRefreshTokens { get; }
    DbSet<Group> Groups { get; }
    DbSet<GroupMember> GroupMembers { get; }
    DbSet<Message> Messages { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    int SaveChanges();
}
