using EzyChat.Application.Models;
using MediatR;

namespace EzyChat.Application.Commands.Groups.DeleteGroup;

public class DeleteGroupCommand : ICommand<AppResponse<Unit>>
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
}
