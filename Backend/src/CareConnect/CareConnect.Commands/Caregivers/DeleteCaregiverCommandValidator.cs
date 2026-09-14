using FluentValidation;

namespace CareConnect.Commands.Caregivers;

public sealed class DeleteCaregiverCommandValidator : AbstractValidator<DeleteCaregiverCommand>
{
    public DeleteCaregiverCommandValidator()
    {
        RuleFor(x => x.CaregiverId).GreaterThan(0);
    }
}
