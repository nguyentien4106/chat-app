using ChatApp.Application.CQRS;
using ChatApp.Application.DTOs.Auth;
using ChatApp.Application.Interfaces.Auth;
using ChatApp.Application.Models;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories;

namespace ChatApp.Application.Commands.Auth.Refresh;

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
