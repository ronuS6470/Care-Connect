using CareConnect.Commands.Auth;
using CareConnect.DTOs.Users;
using FluentValidation;

namespace CareConnect.Commands.Validation.Users;

public class ResetUserPasswordDtoValidator : AbstractValidator<ResetUserPasswordDto>
{
    public ResetUserPasswordDtoValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(PasswordPolicy.MinLength)
            .MaximumLength(PasswordPolicy.MaxLength);
    }
}
