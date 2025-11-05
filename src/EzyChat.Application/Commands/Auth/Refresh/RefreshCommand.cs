using EzyChat.Application.CQRS;
using EzyChat.Application.DTOs.Auth;
using EzyChat.Application.Models;
using FluentValidation;

namespace EzyChat.Application.Commands.Auth.Refresh;

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