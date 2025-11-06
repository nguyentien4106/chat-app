using EzyChat.Api.Controllers.Base;
using EzyChat.Application.DTOs.Common;
using EzyChat.Application.Models;
using EzyChat.Application.Queries.Users.GetUser;
using EzyChat.Application.Queries.Users.SearchUser;

namespace EzyChat.Api.Controllers;

// API/Controllers/UsersController.cs
[ApiController]
[Route("api/[controller]")]
public class UsersController(IMediator mediator) : AuthenticatedControllerBase
{
    [HttpGet("search")]
    public async Task<ActionResult<AppResponse<List<UserDto>>>> SearchUsers([FromQuery] string searchTerm)
    {
        var userId = CurrentUserId;
        
        var query = new SearchUsersQuery
        {
            SearchTerm = searchTerm,
            CurrentUserId = userId
        };

        var response = await mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<AppResponse<UserDto>>> GetUser(Guid userId)
    {
        var response = await mediator.Send(new GetUserQuery { UserId = userId });
        
        if (!response.IsSuccess)
        {
            return NotFound(response);
        }

        return Ok(response);
    }
}