<<<<<<< HEAD
namespace ChatApp.Application.Commands.Groups.JoinGroup;

public class JoinGroupByInviteCommand
{
    
=======
using ChatApp.Application.Models;
using MediatR;

namespace ChatApp.Application.Commands.Groups.JoinGroup;

// Application/Groups/Commands/JoinGroupByInviteCommand.cs
public class JoinGroupByInviteCommand : ICommand<AppResponse<Unit>>
{
    public string InviteCode { get; set; } = string.Empty;
    public Guid UserId { get; set; }
>>>>>>> a957673 (initial)
}