using EzyChat.Application.DTOs.Common;
using EzyChat.Application.Models;

namespace EzyChat.Application.Commands.Groups.CreateGroup;

// Application/Groups/Commands/CreateGroupCommand.cs
public class CreateGroupCommand : ICommand<AppResponse<GroupDto>>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CreatedById { get; set; }
}