using EzyChat.Api.Controllers.Base;
using EzyChat.Application.Commands.Messages.MarkRead;
using EzyChat.Application.DTOs.Common;
using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Models;
using EzyChat.Application.Queries.Conversations.GetConversationMessages;
using EzyChat.Application.Queries.Conversations.GetUserConversations;
using Microsoft.AspNetCore.Authorization;

namespace EzyChat.Api.Controllers;


// API/Controllers/ConversationsController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConversationsController(IMediator mediator) : AuthenticatedControllerBase
{
    [HttpGet]
    public async Task<ActionResult<AppResponse<PagedResult<ConversationDto>>>> GetUserConversations([FromQuery] PaginationRequest request)
    {
        var query = new GetUserConversationsQuery 
        { 
            UserId = CurrentUserId,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            SortOrder = request.SortOrder
        };
        var response = await mediator.Send(query);
        return Ok(response);
    }

    [HttpPost("{conversationId}/mark-read")]
    public async Task<ActionResult<AppResponse<int>>> MarkRead(Guid conversationId)
    {
        var command = new MarkReadCommand
        {
            ConversationId = conversationId,
            CurrentUserId = CurrentUserId
        };

        var response = await mediator.Send(command);
        return Ok(response);
    }

    [HttpGet("{conversationId}/messages")]
    public async Task<ActionResult<AppResponse<PagedResult<MessageDto>>>> GetConversationMessages(
        Guid conversationId,
        [FromQuery] DateTime? beforeDateTime)
    {
        var query = new GetConversationMessagesQuery
        {
            ConversationId = conversationId,
            CurrentUserId = CurrentUserId,
            BeforeDateTime = beforeDateTime ?? DateTime.Now
        };

        var response = await mediator.Send(query);
        return Ok(response);
    }
    
}
