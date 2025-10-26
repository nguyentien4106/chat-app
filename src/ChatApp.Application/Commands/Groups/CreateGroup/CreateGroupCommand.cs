<<<<<<< HEAD
namespace ChatApp.Application.Commands.Groups.CreateGroup;

public class CreateGroupCommand
{
    
=======
using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Models;

namespace ChatApp.Application.Commands.Groups.CreateGroup;

// Application/Groups/Commands/CreateGroupCommand.cs
public class CreateGroupCommand : ICommand<AppResponse<GroupDto>>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CreatedById { get; set; }
>>>>>>> a957673 (initial)
}