using EzyChat.Application.CQRS;
using EzyChat.Application.DTOs.Auth;
using EzyChat.Application.Interfaces.Auth;
using EzyChat.Application.Models;
using EzyChat.Domain.Entities;
using EzyChat.Domain.Repositories;

namespace EzyChat.Application.Commands.Auth.Refresh;

public class RefreshCommandHandler(
    IRefreshTokenValidatorService tokenValidatorService,
    IAuthenticateService authenticateService,
    IRepository<UserRefreshToken> refreshTokenRepository
    ) : ICommandHandler<RefreshCommand, AppResponse<AuthenticateResponse>>
{
    public async Task<AppResponse<AuthenticateResponse>> Handle(RefreshCommand command, CancellationToken cancellationToken)
    {
        if (!tokenValidatorService.Validate(command.RefreshToken))
        {
            return AppResponse<AuthenticateResponse>.Error("Invalid refresh token.");
        }

        var userRefreshToken = await refreshTokenRepository.GetSingleAsync(
            urt => urt.RefreshToken == command.RefreshToken,
            ["ApplicationUser"],
            cancellationToken
        );

        return await authenticateService.Authenticate(userRefreshToken.ApplicationUser, cancellationToken);
    }
}
