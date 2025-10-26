using ChatApp.Application.CQRS;
using ChatApp.Application.DTOs.Auth;
using ChatApp.Application.Interfaces.Auth;
using ChatApp.Application.Models;
using ChatApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Application.Commands.Auth.Login;

/// <summary>
/// Handles user login without throwing exceptions.
/// Returns appropriate AppResponse with success/error status and messages.
/// Validates email, password, account status, and generates JWT tokens on successful login.
/// </summary>
public class LoginCommandHandler(
    UserManager<ApplicationUser> userManager,
    IAuthenticateService authenticateService
    ) : ICommandHandler<LoginCommand, AppResponse<AuthenticateResponse>>
{
    public async Task<AppResponse<AuthenticateResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Find user by email
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return AppResponse<AuthenticateResponse>.Error($"{request.Email} is not found.");
        }

        // Check if the user's email is confirmed (if required)
        if (!await userManager.IsEmailConfirmedAsync(user))
        {
            return AppResponse<AuthenticateResponse>.Error("Email not confirmed. Please check your email and confirm your account.");
        }

        // Check if the user account is locked out
        if (await userManager.IsLockedOutAsync(user))
        {
            return AppResponse<AuthenticateResponse>.Error("Account is locked out. Please try again later.");
        }

        // Validate password
        var passwordValid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            // Record failed login attempt
            await userManager.AccessFailedAsync(user);
            return AppResponse<AuthenticateResponse>.Error("Password is incorrect.");
        }

        // Reset failed login attempts on successful login
        await userManager.ResetAccessFailedCountAsync(user);

        // Generate tokens
        return await authenticateService.Authenticate(user, cancellationToken);
    }
}