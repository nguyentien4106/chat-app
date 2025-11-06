using EzyChat.Api.Controllers.Base;
using EzyChat.Application.Commands.Messages.SendMessage;
using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Models;
using EzyChat.Application.Queries.Messages.GetConversationMessages;
using EzyChat.Application.Queries.Messages.GetGroupMessages;
using Microsoft.AspNetCore.Authorization;

namespace EzyChat.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessagesController(IMediator mediator) : AuthenticatedControllerBase
{
    [HttpGet("conversation/{otherUserId}")]
    public async Task<ActionResult<AppResponse<List<MessageDto>>>> GetConversationMessages(
        Guid otherUserId, 
        [FromQuery] int skip = 0, 
        [FromQuery] int take = 50)
    {
        var userId = CurrentUserId;
        var query = new GetConversationMessagesQuery()
        {
            User1Id = userId,
            User2Id = otherUserId,
            Skip = skip,
            Take = take
        };

        var response = await mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("group/{groupId}")]
    public async Task<ActionResult<AppResponse<List<MessageDto>>>> GetGroupMessages(
        Guid groupId, 
        [FromQuery] int skip = 0, 
        [FromQuery] int take = 50)
    {
        var query = new GetGroupMessagesQuery()
        {
            GroupId = groupId,
            Skip = skip,
            Take = take
        };

        var response = await mediator.Send(query);
        return Ok(response);
    }

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
