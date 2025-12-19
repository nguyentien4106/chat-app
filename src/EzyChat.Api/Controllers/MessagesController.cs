using EzyChat.Api.Controllers.Base;
using EzyChat.Application.Commands.Messages.SendMessage;
using EzyChat.Application.Commands.Messages.PinMessage;
using EzyChat.Application.Commands.Messages.UnpinMessage;
using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Models;
using EzyChat.Application.Queries.Messages;
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

    [HttpPost("pin")]
    public async Task<ActionResult<AppResponse<PinMessageDto>>> PinMessage([FromBody] PinMessageRequest request)
    {
        var command = new PinMessageCommand
        {
            MessageId = request.MessageId,
            ConversationId = request.ConversationId,
            GroupId = request.GroupId,
            PinnedByUserId = CurrentUserId
        };

        var response = await mediator.Send(command);
        return Ok(response);
    }

    [HttpDelete("unpin")]
    public async Task<ActionResult<AppResponse<bool>>> UnpinMessage([FromQuery] Guid messageId, [FromQuery] Guid? conversationId, [FromQuery] Guid? groupId)
    {
        var command = new UnpinMessageCommand
        {
            MessageId = messageId,
            ConversationId = conversationId,
            GroupId = groupId,
            UnpinnedByUserId = CurrentUserId
        };

        var response = await mediator.Send(command);
        return Ok(response);
    }

    [HttpGet("pinned")]
    public async Task<ActionResult<AppResponse<List<PinMessageDto>>>> GetPinnedMessages([FromQuery] Guid? conversationId, [FromQuery] Guid? groupId)
    {
        var query = new GetPinnedMessagesQuery
        {
            ConversationId = conversationId,
            GroupId = groupId
        };

        var response = await mediator.Send(query);
        return Ok(response);
    }

}
