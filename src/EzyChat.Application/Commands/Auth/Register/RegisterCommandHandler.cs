using EzyChat.Application.CQRS;
using EzyChat.Domain.Entities;
using EzyChat.Domain.Enums;
using EzyChat.Application.Models;
using Microsoft.AspNetCore.Identity;

namespace EzyChat.Application.Commands.Auth.Register;

public class RegisterCommandHandler(UserManager<ApplicationUser> userManager) : ICommandHandler<RegisterCommand, AppResponse<string>>
{
    public async Task<AppResponse<string>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = command.RegisterRequest.Email.Split("@")[0],
            Email = command.RegisterRequest.Email,
            FirstName = command.RegisterRequest.FirstName,
            LastName = command.RegisterRequest.LastName,
            PhoneNumber = command.RegisterRequest.PhoneNumber,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
        };

        var result = await userManager.CreateAsync(user, command.RegisterRequest.Password);
        if (!result.Succeeded)
        {
            return AppResponse<string>.Error(result.Errors.Select(e => e.Description));
        }

        await userManager.AddToRoleAsync(user, nameof(Roles.User));

        return AppResponse<string>.Success("User registered successfully.");
    }
}
