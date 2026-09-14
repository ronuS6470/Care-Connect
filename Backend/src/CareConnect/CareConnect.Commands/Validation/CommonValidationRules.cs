using System.Text.RegularExpressions;
using FluentValidation;

namespace CareConnect.Commands.Validation;

/// <summary>
/// Shared FluentValidation rule chains for value shapes that recur across multiple DTOs
/// (phone numbers, ZIP codes, password strength), so the rules — and their wording — stay
/// identical everywhere they're used instead of drifting copy to copy.
/// </summary>
public static class CommonValidationRules
{
    private static readonly Regex UsPhoneRegex =
        new(@"^\+?1?[-.\s]?\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}$", RegexOptions.Compiled);

    private static readonly Regex UsZipCodeRegex =
        new(@"^\d{5}(-\d{4})?$", RegexOptions.Compiled);

    public static IRuleBuilderOptions<T, string?> MustBeAValidUsPhoneNumber<T>(this IRuleBuilder<T, string?> ruleBuilder) =>
        ruleBuilder.Matches(UsPhoneRegex).WithMessage("'{PropertyName}' must be a valid US phone number.");

    public static IRuleBuilderOptions<T, string> MustBeAValidUsZipCode<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.Matches(UsZipCodeRegex).WithMessage("'{PropertyName}' must be a valid US ZIP code (12345 or 12345-6789).");

    public static IRuleBuilderOptions<T, string> MustBeAStrongPassword<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .MinimumLength(8).WithMessage("'{PropertyName}' must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("'{PropertyName}' must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("'{PropertyName}' must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("'{PropertyName}' must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("'{PropertyName}' must contain at least one special character.");
}
