using EzyChat.Application.DTOs.Messages;

namespace EzyChat.Application.Interfaces;

public interface IMessageRepository
{
    Task<PagedResult<MessageDto>> GetPagedResultAsync(
        DateTime beforeDateTime,
        Guid id,
        string type = "group",
        int pageSize = 20,
        string[]? includeProperties = null,
        CancellationToken cancellationToken = default
    );
}