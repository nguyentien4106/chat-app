using ChatApp.Application.CQRS;
using ChatApp.Application.DTOs.Auth;
using ChatApp.Application.Models;
using FluentValidation;

namespace ChatApp.Application.Commands.Auth.Login;

public record LoginCommand(string Email, string Password) : ICommand<AppResponse<AuthenticateResponse>>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Email is invalid.")
            .NotEmpty();

        RuleFor(x => x.Password).NotEmpty();
    }
}