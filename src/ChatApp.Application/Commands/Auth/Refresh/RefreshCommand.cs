using ChatApp.Application.CQRS;
using ChatApp.Application.DTOs.Auth;
using ChatApp.Application.Models;
using FluentValidation;

namespace ChatApp.Application.Commands.Auth.Refresh;

public record RefreshCommand(string RefreshToken) : ICommand<AppResponse<AuthenticateResponse>>;

public class RefreshCommandValidator : AbstractValidator<RefreshCommand>
{
    public RefreshCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required.");
    }
}