using System.Security.Claims;
using EzyChat.Application.CQRS;
using EzyChat.Application.Models;
using FluentValidation;

namespace EzyChat.Application.Commands.Auth.Logout;

public record LogoutCommand(ClaimsPrincipal User) : ICommand<AppResponse<bool>>;

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.User).NotNull();
    }
}