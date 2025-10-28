using ChatApp.Application.Models;
using MediatR;

namespace ChatApp.Application.Commands.Groups.LeaveGroup;

public class LeaveGroupCommand : ICommand<AppResponse<Unit>>
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
}
