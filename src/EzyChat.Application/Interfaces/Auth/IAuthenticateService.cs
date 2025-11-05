using EzyChat.Application.DTOs.Auth;
using EzyChat.Application.Models;
using EzyChat.Domain.Entities;

namespace EzyChat.Application.Interfaces.Auth;

public interface IAuthenticateService
{
    Task<AppResponse<AuthenticateResponse>> Authenticate(ApplicationUser user, CancellationToken cancellationToken);
}