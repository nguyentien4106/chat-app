using ChatApp.Application.Models;
using MediatR;

namespace ChatApp.Application.Commands.Groups.AddUserToGroup;

// Application/Groups/Commands/AddMemberToGroupCommand.cs
public class AddMemberToGroupCommand : ICommand<AppResponse<Unit>>
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public Guid AddedById { get; set; }
}
