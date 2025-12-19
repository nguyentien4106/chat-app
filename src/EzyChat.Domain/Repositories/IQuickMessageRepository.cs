using EzyChat.Domain.Entities;

namespace EzyChat.Domain.Repositories;

public interface IQuickMessageRepository : IRepository<QuickMessage>
{
    Task<IEnumerable<QuickMessage>> GetUserQuickMessagesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<QuickMessage?> GetByKeyAsync(string key, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> KeyExistsAsync(string key, Guid userId, CancellationToken cancellationToken = default);
}
