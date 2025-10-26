using ChatApp.Application.Interfaces.Auth;
using ChatApp.Application.Settings;
using ChatApp.Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Infrastructure.Services.Auth;

public class AccessTokenService(ITokenGeneratorService tokenGenerator, JwtSettings jwtSettings, UserManager<ApplicationUser> userManager) : IAccessTokenService
{
    public async Task<string> GetTokenAsync(ApplicationUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var rolesClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));
        List<Claim> claims =
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim("userName", user.UserName),
                ..rolesClaims
            ];

        return tokenGenerator.Generate(jwtSettings.AccessTokenSecret, jwtSettings.AccessTokenExpirationMinutes, claims);
    }
}
