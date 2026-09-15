using CareConnect.DTOs.Auth;
using FluentValidation;

namespace CareConnect.Commands.Validation.Auth;

/// <summary>
/// Shape only — that an email is well-formed and a password was supplied at all. Whether the
/// credentials are *correct* is decided in LoginCommandHandler, and is deliberately never
/// reported at field level.
/// </summary>
public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(128);
    }
}
