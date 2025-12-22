using EzyChat.Application.DTOs.QuickMessages;
using EzyChat.Application.Models;

namespace EzyChat.Application.Commands.QuickMessages.CreateQuickMessage;

public class CreateQuickMessageCommand : ICommand<AppResponse<QuickMessageDto>>
{
    public string Content { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}
