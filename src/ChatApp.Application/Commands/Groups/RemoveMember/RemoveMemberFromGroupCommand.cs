using ChatApp.Application.Models;
using MediatR;

namespace ChatApp.Application.Commands.Groups.RemoveMember;

public class RemoveMemberFromGroupCommand : ICommand<AppResponse<Unit>>
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public Guid RemovedById { get; set; }
}
