using EzyChat.Application.CQRS;
using EzyChat.Application.DTOs.Auth;
using EzyChat.Application.Models;
using FluentValidation;

namespace EzyChat.Application.Commands.Auth.Login;

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