using FluentValidation;

namespace CareConnect.Commands.Caregivers;

public sealed class ActivateCaregiverCommandValidator : AbstractValidator<ActivateCaregiverCommand>
{
    public ActivateCaregiverCommandValidator()
    {
        RuleFor(x => x.CaregiverId).GreaterThan(0);
    }
}
