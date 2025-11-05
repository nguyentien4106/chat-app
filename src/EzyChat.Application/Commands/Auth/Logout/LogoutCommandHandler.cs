using EzyChat.Application.CQRS;
using EzyChat.Application.Interfaces;
using EzyChat.Application.Models;
using EzyChat.Domain.Entities;
using EzyChat.Domain.Repositories;
using Microsoft.AspNetCore.Identity;

namespace EzyChat.Application.Commands.Auth.Logout;

public class LogoutCommandHandler(
    UserManager<ApplicationUser> userManager, 
    IRepository<UserRefreshToken> refreshTokenRepository
    ) : ICommandHandler<LogoutCommand, AppResponse<bool>>
{
    public async Task<AppResponse<bool>> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        if (!(command.User.Identity?.IsAuthenticated ?? false))
        {
            return AppResponse<bool>.Success(true);
        }
        
        var idClaim = command.User.Claims.FirstOrDefault(x => x.Type == "Id");
        if (idClaim == null)
        {
            return AppResponse<bool>.Success(false);
        }
        
        if (!Guid.TryParse(idClaim.Value, out var userId))
        {
            return AppResponse<bool>.Success(false);
        }
        
        var appUser = await userManager.GetUserAsync(command.User);
        if (appUser == null)
        {
            return AppResponse<bool>.Success(false);
        }

        await userManager.UpdateSecurityStampAsync(appUser);
        await refreshTokenRepository.DeleteByIdAsync(appUser.Id, cancellationToken);

        return AppResponse<bool>.Success(true);
    }
}