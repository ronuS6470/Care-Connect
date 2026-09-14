using FluentValidation;

namespace CareConnect.Commands.Caregivers;

public sealed class DeactivateCaregiverCommandValidator : AbstractValidator<DeactivateCaregiverCommand>
{
    public DeactivateCaregiverCommandValidator()
    {
        RuleFor(x => x.CaregiverId).GreaterThan(0);
    }
}
