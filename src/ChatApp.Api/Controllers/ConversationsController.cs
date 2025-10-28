using System.Security.Claims;
using ChatApp.Api.Controllers.Base;
using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Models;
using ChatApp.Application.Queries.Groups.GetUserConversations;
using Microsoft.AspNetCore.Authorization;

namespace ChatApp.Api.Controllers;


// API/Controllers/ConversationsController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConversationsController(IMediator mediator) : AuthenticatedControllerBase
{
    [HttpGet]
    public async Task<ActionResult<AppResponse<List<ConversationDto>>>> GetUserConversations()
    {
        var query = new GetUserConversationsQuery { UserId = CurrentUserId };
        var response = await mediator.Send(query);
        return Ok(response);
    }

    // [HttpPost]
    // public async Task<ActionResult<AppResponse<ConversationDto>>> StartConversation([FromBody] StartConversationCommand command)
    // {
    //     command.UserId = CurrentUserId;
    //     var response = await mediator.Send(command);
    //     return Ok(response);
    // }
}
