using EzyChat.Application.DTOs.QuickMessages;
using FluentValidation;

namespace EzyChat.Application.Validators.QuickMessages;

public class CreateQuickMessageDtoValidator : AbstractValidator<CreateQuickMessageDto>
{
    public CreateQuickMessageDtoValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty().WithMessage("Key is required")
            .MaximumLength(50).WithMessage("Key cannot exceed 50 characters")
            .Matches(@"^[a-zA-Z0-9_-]+$").WithMessage("Key can only contain letters, numbers, underscores, and hyphens");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(1000).WithMessage("Content cannot exceed 1000 characters");
    }
}
