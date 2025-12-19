using EzyChat.Domain.Entities;
using EzyChat.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EzyChat.Infrastructure.Repositories;

public class QuickMessageRepository(EzyChatDbContext dbContext, ILogger<QuickMessageRepository> logger) 
    : EfRepository<QuickMessage>(dbContext, logger), IQuickMessageRepository
{
    public async Task<IEnumerable<QuickMessage>> GetUserQuickMessagesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbSet
            .Where(qm => qm.UserId == userId)
            .OrderBy(qm => qm.Key)
            .ToListAsync(cancellationToken);
    }

    public async Task<QuickMessage?> GetByKeyAsync(string key, Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbSet
            .FirstOrDefaultAsync(qm => qm.Key.StartsWith(key) && qm.UserId == userId, cancellationToken);
    }

    public async Task<bool> KeyExistsAsync(string key, Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbSet
            .AnyAsync(qm => qm.Key == key && qm.UserId == userId, cancellationToken);
    }
}
