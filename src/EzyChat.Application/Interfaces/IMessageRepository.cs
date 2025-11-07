using EzyChat.Application.DTOs.Messages;

namespace EzyChat.Application.Interfaces;

public interface IMessageRepository
{
    Task<PagedResult<MessageDto>> GetPagedResultAsync(
        DateTime beforeDateTime,
        Guid groupId,
        int pageSize = 20,
        string[]? includeProperties = null,
        CancellationToken cancellationToken = default
    );
}