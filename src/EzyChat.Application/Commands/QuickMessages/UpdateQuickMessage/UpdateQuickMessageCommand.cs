using EzyChat.Application.DTOs.QuickMessages;
using EzyChat.Application.Models;

namespace EzyChat.Application.Commands.QuickMessages.UpdateQuickMessage;

public class UpdateQuickMessageCommand : ICommand<AppResponse<QuickMessageDto>>
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}
