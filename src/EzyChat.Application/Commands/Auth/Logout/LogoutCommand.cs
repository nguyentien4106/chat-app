using FluentValidation;

namespace EzyChat.Application.Commands.Auth.Logout;

public record LogoutCommand(Guid UserId) : ICommand<AppResponse<bool>>;

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}