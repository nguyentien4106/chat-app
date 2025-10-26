<<<<<<< HEAD
namespace ChatApp.Application.Commands.Groups.GenerateLink;

public class GenerateLinkCommand
{
    
=======
using ChatApp.Application.Models;

namespace ChatApp.Application.Commands.Groups.GenerateLink;

public class GenerateLinkCommand : ICommand<AppResponse<string>>
{
    public Guid GroupId { get; set; }
    public Guid RequestedById { get; set; }
>>>>>>> a957673 (initial)
}