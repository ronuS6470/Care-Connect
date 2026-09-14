using FluentValidation;

namespace CareConnect.Commands.Availability;

public sealed class DeleteAvailabilityCommandValidator : AbstractValidator<DeleteAvailabilityCommand>
{
    public DeleteAvailabilityCommandValidator()
    {
        RuleFor(x => x.AvailabilityId).GreaterThan(0);
    }
}
