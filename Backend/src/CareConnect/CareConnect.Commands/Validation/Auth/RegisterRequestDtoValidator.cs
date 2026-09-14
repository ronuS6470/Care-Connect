using CareConnect.DTOs.Auth;
using FluentValidation;

namespace CareConnect.Commands.Validation.Auth;

public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        // EmailNormalizer.Normalize() is applied by the command handler before persisting/comparing;
        // normalization doesn't change format validity, so it isn't needed for this check.
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(256)
            .EmailAddress()
            .WithMessage("'{PropertyName}' must be a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MustBeAStrongPassword();

        RuleFor(x => x.PhoneNumber)
            .MustBeAValidUsPhoneNumber()
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.Role)
            .IsInEnum();
    }
}
