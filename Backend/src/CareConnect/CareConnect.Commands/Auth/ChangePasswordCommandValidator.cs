using CareConnect.DTOs.Auth;
using FluentValidation;

namespace CareConnect.Commands.Auth;

/// <summary>Delegates to the existing ChangePasswordDto validator rather than duplicating its rules.</summary>
public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator(IValidator<ChangePasswordDto> dtoValidator)
    {
        RuleFor(x => x.Password).SetValidator(dtoValidator);
    }
}
