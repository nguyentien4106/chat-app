using EzyChat.Application.DTOs.Messages;

namespace EzyChat.Application.Interfaces;

public interface IMessagesService
{
    
    Task<AppResponse<PagedResult<MessageDto>>> GetGroupMessagesAsync(
        DateTime dateTime,
        CancellationToken cancellationToken = default
    );
}