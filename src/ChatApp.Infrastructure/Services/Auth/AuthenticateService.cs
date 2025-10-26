using ChatApp.Application.DTOs.Auth;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Auth;
using ChatApp.Application.Models;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Exceptions;
using ChatApp.Domain.Repositories;

namespace ChatApp.Infrastructure.Services.Auth;

public class AuthenticateService(
    IAccessTokenService accessTokenService, 
    IRefreshTokenService refreshTokenService, 
    IRepository<UserRefreshToken> refreshTokenRepository) : IAuthenticateService
{
    public async Task<AppResponse<AuthenticateResponse>> Authenticate(ApplicationUser user, CancellationToken cancellationToken)
    {
        var accessToken = await accessTokenService.GetTokenAsync(user);
        var refreshToken = await refreshTokenService.GetTokenAsync(user);
        UserRefreshToken currentUserRefreshToken;
        bool success;

        try {
            currentUserRefreshToken = await refreshTokenRepository.GetSingleAsync(rf => rf.ApplicationUserId == user.Id, cancellationToken: cancellationToken);
            currentUserRefreshToken.RefreshToken = refreshToken;
            success = await refreshTokenRepository.UpdateAsync(currentUserRefreshToken, cancellationToken);
        }
        catch(NotFoundException ex){
            var newUserRefreshToken = new UserRefreshToken
            {
                ApplicationUserId = user.Id,
                RefreshToken = refreshToken
            };
            success = await refreshTokenRepository.AddAsync(newUserRefreshToken, cancellationToken) != null;
        }
        
        return success ?
            AppResponse<AuthenticateResponse>.Success(new AuthenticateResponse(accessToken, refreshToken)) :
            AppResponse<AuthenticateResponse>.Error("Failed to save refresh token.");
    }
}