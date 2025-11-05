using EzyChat.Application.Models;

namespace EzyChat.Application.Commands.Groups.GenerateLink;

public class GenerateLinkCommand : ICommand<AppResponse<string>>
{
    public Guid GroupId { get; set; }
    public Guid RequestedById { get; set; }
}