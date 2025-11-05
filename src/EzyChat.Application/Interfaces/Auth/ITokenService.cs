using EzyChat.Domain.Entities;

namespace EzyChat.Application.Interfaces.Auth;

public interface ITokenService
{
    Task<string> GetTokenAsync(ApplicationUser user);
}
