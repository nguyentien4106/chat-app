using EzyChat.Api.Controllers.Base;
using EzyChat.Application.Commands.Messages.SendMessage;
using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Models;
using Microsoft.AspNetCore.Authorization;

namespace EzyChat.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessagesController(IMediator mediator) : AuthenticatedControllerBase
{

    [HttpPost]
    public async Task<ActionResult<AppResponse<MessageDto>>> SendMessage([FromBody] SendMessageRequest request)
    {
        var userId = CurrentUserId;
        var command = new SendMessageCommand
        {
            SenderId = userId,
            ConversationId = request.ConversationId,
            GroupId = request.GroupId,
            Content = request.Content
        };

        var response = await mediator.Send(command);
        return Ok(response);
    }

}
