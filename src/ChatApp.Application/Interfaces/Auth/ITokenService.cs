using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces.Auth;

public interface ITokenService
{
    Task<string> GetTokenAsync(ApplicationUser user);
}
