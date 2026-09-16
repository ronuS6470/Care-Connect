using FluentValidation;

namespace CareConnect.Commands.Users;

/// <summary>No DTO to delegate to — IsActive comes from the route, not a body.</summary>
public sealed class SetUserActiveCommandValidator : AbstractValidator<SetUserActiveCommand>
{
    public SetUserActiveCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0);
    }
}
