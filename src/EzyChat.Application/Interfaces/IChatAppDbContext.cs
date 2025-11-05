using EzyChat.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EzyChat.Application.Interfaces;

public interface IEzyChatDbContext
{
    DbSet<ApplicationUser> Users { get; }
    DbSet<UserRefreshToken> UserRefreshTokens { get; }
    DbSet<Group> Groups { get; }
    DbSet<GroupMember> GroupMembers { get; }
    DbSet<Message> Messages { get; }
    DbSet<Conversation> Conversations { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
}
