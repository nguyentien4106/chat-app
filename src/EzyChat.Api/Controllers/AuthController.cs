using EzyChat.Application.Commands.Auth.Login;
using EzyChat.Application.Commands.Auth.Logout;
using EzyChat.Application.Commands.Auth.Refresh;
using EzyChat.Application.Commands.Auth.Register;
using EzyChat.Application.DTOs.Auth;
using EzyChat.Application.Models;
using Microsoft.AspNetCore.Authorization;

namespace EzyChat.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthController(IMediator _bus) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<AppResponse<AuthenticateResponse>>> Login(LoginRequest loginRequest, CancellationToken cancellationToken = new())
    {
        var command = new LoginCommand(loginRequest.UserName, loginRequest.Password);
        return await _bus.Send(command, cancellationToken);
    }

    [HttpPost]
    public async Task<ActionResult<AppResponse<string>>> Register(RegisterRequest registerRequest, CancellationToken cancellationToken = new())
    {
        var command = new RegisterCommand(registerRequest);  
        return Ok(await _bus.Send(command, cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Refresh(RefreshRequest refreshRequest, CancellationToken cancellationToken = new())
    {
       return Ok(await _bus.Send(new RefreshCommand(refreshRequest.RefreshToken), cancellationToken));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<AppResponse<bool>>> Logout(CancellationToken cancellationToken = new())
    {
        
        return Ok(await _bus.Send(new LogoutCommand(User), cancellationToken));
    }

    [Authorize]
    [HttpGet("me")]
    public Task<ActionResult> Me(CancellationToken cancellationToken = new())
    {

        return Task.FromResult<ActionResult>(Ok(User));
    }
}
