using ChatApp.Application.CQRS;
using ChatApp.Application.DTOs.Auth;
using ChatApp.Application.Models;
using FluentValidation;

namespace ChatApp.Application.Commands.Auth.Register;

public record RegisterCommand(RegisterRequest RegisterRequest) : ICommand<AppResponse<string>>;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.RegisterRequest.Email)
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .NotEmpty()
            .WithMessage("Username is required.")
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters long.");

        RuleFor(x => x.RegisterRequest.Password)
            .NotEmpty()
            .WithMessage("Password is required.");

        RuleFor(x => x.RegisterRequest.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.")
            .MaximumLength(50)
            .WithMessage("First name must not exceed 50 characters.");

        RuleFor(x => x.RegisterRequest.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .MaximumLength(50)
            .WithMessage("Last name must not exceed 50 characters.");
    }
}
