using System.Security.Claims;
using ChatApp.Api.Controllers.Base;
using ChatApp.Application.Commands.Messages.MarkRead;
using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Models;
using ChatApp.Application.Queries.Conversations.GetMessagesByConversationId;
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

    [HttpPost("{conversationId}/mark-read/{senderId}")]
    public async Task<ActionResult<AppResponse<int>>> MarkRead(Guid conversationId, Guid senderId)
    {
        var command = new MarkReadCommand
        {
            ConversationId = conversationId,
            SenderId = senderId,
            CurrentUserId = CurrentUserId
        };

        var response = await mediator.Send(command);
        return Ok(response);
    }

    [HttpGet("{conversationId}/messages")]
    public async Task<ActionResult<AppResponse<PagedResult<MessageDto>>>> GetConversationMessages(
        Guid conversationId,
        [FromQuery] PaginationRequest request)
    {
        var query = new GetMessagesByConversationIdQuery
        {
            ConversationId = conversationId,
            CurrentUserId = CurrentUserId,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            SortOrder = request.SortOrder
        };

        var response = await mediator.Send(query);
        return Ok(response);
    }
    
}
