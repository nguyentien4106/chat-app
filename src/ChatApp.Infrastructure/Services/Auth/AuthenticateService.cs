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
        var currentUserRefreshToken = await refreshTokenRepository.GetSingleAsync(rf => rf.ApplicationUserId == user.Id, cancellationToken: cancellationToken);

        var refreshTokenEntity = currentUserRefreshToken == null ? new UserRefreshToken
        {
            Id = Guid.NewGuid(),
            ApplicationUserId = user.Id,
        } : currentUserRefreshToken;

        refreshTokenEntity.RefreshToken = refreshToken;

        await refreshTokenRepository.AddOrUpdateAsync(refreshTokenEntity);

        return AppResponse<AuthenticateResponse>.Success(new AuthenticateResponse(accessToken, refreshToken));
    }
}