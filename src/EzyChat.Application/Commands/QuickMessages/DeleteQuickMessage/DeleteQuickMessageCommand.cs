using EzyChat.Application.Models;

namespace EzyChat.Application.Commands.QuickMessages.DeleteQuickMessage;

public class DeleteQuickMessageCommand : ICommand<AppResponse<bool>>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}
