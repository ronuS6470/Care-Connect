using CareConnect.Commands.Auth;
using CareConnect.DTOs.Auth;
using FluentValidation;

namespace CareConnect.Commands.Validation.Auth;

/// <summary>
/// Shape only. Whether CurrentPassword is *correct* is decided in ChangePasswordCommandHandler —
/// checking it here would mean hashing before the pipeline knows who is calling.
/// </summary>
public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(PasswordPolicy.MinLength)
            .MaximumLength(PasswordPolicy.MaxLength);
    }
}
