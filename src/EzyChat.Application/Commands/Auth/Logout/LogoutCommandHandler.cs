using Microsoft.AspNetCore.Identity;

namespace EzyChat.Application.Commands.Auth.Logout;

public class LogoutCommandHandler(
    UserManager<ApplicationUser> userManager, 
    IRepository<UserRefreshToken> refreshTokenRepository
    ) : ICommandHandler<LogoutCommand, AppResponse<bool>>
{
    public async Task<AppResponse<bool>> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        var appUser = await userManager.FindByIdAsync(command.UserId.ToString());
        if (appUser == null)
        {
            return AppResponse<bool>.Error("User not found.");
        }

        await userManager.UpdateSecurityStampAsync(appUser);
        var refreshToken = await refreshTokenRepository.GetSingleAsync(rf => rf.ApplicationUserId == appUser.Id, cancellationToken: cancellationToken);

        if (refreshToken != null)
        {
            await refreshTokenRepository.DeleteAsync(refreshToken, cancellationToken);
        }

        return AppResponse<bool>.Success(true);
    }
}