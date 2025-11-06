namespace EzyChat.Application.Interfaces;

public interface ISignalRService
{
    Task NotifyGroupAsync(string connectionId, string action, object data, CancellationToken cancellationToken = default);
    Task NotifyUserAsync(string connectionId, string action, object data, CancellationToken cancellationToken = default);
    
}