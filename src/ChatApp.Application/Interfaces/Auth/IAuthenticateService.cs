using ChatApp.Application.DTOs.Auth;
using ChatApp.Application.Models;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces.Auth;

public interface IAuthenticateService
{
    Task<AppResponse<AuthenticateResponse>> Authenticate(ApplicationUser user, CancellationToken cancellationToken);
}