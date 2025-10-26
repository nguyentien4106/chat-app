using System.Security.Claims;
using ChatApp.Application.CQRS;
using ChatApp.Application.Models;
using FluentValidation;

namespace ChatApp.Application.Commands.Auth.Logout;

public record LogoutCommand(ClaimsPrincipal User) : ICommand<AppResponse<bool>>;

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.User).NotNull();
    }
}