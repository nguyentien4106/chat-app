<<<<<<< HEAD
namespace ChatApp.Api.Controllers;

public class UsersController
{
=======
using ChatApp.Api.Controllers.Base;
using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Models;
using ChatApp.Application.Queries.Users.GetUser;
using ChatApp.Application.Queries.Users.SearchUser;

namespace ChatApp.Api.Controllers;

// API/Controllers/UsersController.cs
[ApiController]
[Route("api/[controller]")]
public class UsersController : AuthenticatedControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("search")]
    public async Task<ActionResult<AppResponse<List<UserDto>>>> SearchUsers([FromQuery] string searchTerm)
    {
        var userId = CurrentUserId;
        
        var query = new SearchUsersQuery
        {
            SearchTerm = searchTerm,
            CurrentUserId = userId
        };

        var response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<AppResponse<UserDto>>> GetUser(Guid userId)
    {
        var response = await _mediator.Send(new GetUserQuery { UserId = userId });
        
        if (!response.IsSuccess)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

>>>>>>> a957673 (initial)
    
}