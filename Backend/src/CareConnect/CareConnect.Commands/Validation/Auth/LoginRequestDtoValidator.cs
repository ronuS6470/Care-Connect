using CareConnect.DTOs.Auth;
using FluentValidation;

namespace CareConnect.Commands.Validation.Auth;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("'{PropertyName}' must be a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
