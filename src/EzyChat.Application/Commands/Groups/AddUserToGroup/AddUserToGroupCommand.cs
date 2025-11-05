using EzyChat.Application.Models;
using MediatR;

namespace EzyChat.Application.Commands.Groups.AddUserToGroup;

// Application/Groups/Commands/AddMemberToGroupCommand.cs
public class AddMemberToGroupCommand : ICommand<AppResponse<Unit>>
{
    public Guid GroupId { get; set; }
    public string UserName { get; set; }
    public Guid AddedById { get; set; }
}
