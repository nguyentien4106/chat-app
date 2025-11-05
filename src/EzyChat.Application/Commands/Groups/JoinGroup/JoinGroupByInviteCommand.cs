using EzyChat.Application.Models;
using MediatR;

namespace EzyChat.Application.Commands.Groups.JoinGroup;

// Application/Groups/Commands/JoinGroupByInviteCommand.cs
public class JoinGroupByInviteCommand : ICommand<AppResponse<Unit>>
{
    public string InviteCode { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}