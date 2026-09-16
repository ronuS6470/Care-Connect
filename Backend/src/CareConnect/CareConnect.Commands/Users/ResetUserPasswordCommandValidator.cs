using CareConnect.DTOs.Users;
using FluentValidation;

namespace CareConnect.Commands.Users;

public sealed class ResetUserPasswordCommandValidator : AbstractValidator<ResetUserPasswordCommand>
{
    public ResetUserPasswordCommandValidator(IValidator<ResetUserPasswordDto> dtoValidator)
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0);

        RuleFor(x => x.Password)
            .SetValidator(dtoValidator);
    }
}
