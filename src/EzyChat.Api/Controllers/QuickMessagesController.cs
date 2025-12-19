using EzyChat.Api.Controllers.Base;
using EzyChat.Application.Commands.QuickMessages.CreateQuickMessage;
using EzyChat.Application.Commands.QuickMessages.UpdateQuickMessage;
using EzyChat.Application.Commands.QuickMessages.DeleteQuickMessage;
using EzyChat.Application.DTOs.QuickMessages;
using EzyChat.Application.Models;
using EzyChat.Application.Queries.QuickMessages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EzyChat.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuickMessagesController(IMediator mediator) : AuthenticatedControllerBase
{
    [HttpGet]
    public async Task<ActionResult<AppResponse<List<QuickMessageDto>>>> GetUserQuickMessages()
    {
        var query = new GetUserQuickMessagesQuery
        {
            UserId = CurrentUserId
        };

        var response = await mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("{key}")]
    public async Task<ActionResult<AppResponse<QuickMessageDto>>> GetQuickMessageByKey(string key)
    {
        var query = new GetQuickMessageByKeyQuery
        {
            Key = key,
            UserId = CurrentUserId
        };

        var response = await mediator.Send(query);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<AppResponse<QuickMessageDto>>> CreateQuickMessage([FromBody] CreateQuickMessageDto dto)
    {
        var command = new CreateQuickMessageCommand
        {
            Content = dto.Content,
            Key = dto.Key,
            UserId = CurrentUserId
        };

        var response = await mediator.Send(command);
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AppResponse<QuickMessageDto>>> UpdateQuickMessage(Guid id, [FromBody] UpdateQuickMessageDto dto)
    {
        var command = new UpdateQuickMessageCommand
        {
            Id = id,
            Content = dto.Content,
            Key = dto.Key,
            UserId = CurrentUserId
        };

        var response = await mediator.Send(command);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<AppResponse<bool>>> DeleteQuickMessage(Guid id)
    {
        var command = new DeleteQuickMessageCommand
        {
            Id = id,
            UserId = CurrentUserId
        };

        var response = await mediator.Send(command);
        return Ok(response);
    }
}
