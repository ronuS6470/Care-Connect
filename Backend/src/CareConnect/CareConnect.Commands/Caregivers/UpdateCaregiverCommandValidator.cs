using CareConnect.DTOs.Caregivers;
using FluentValidation;

namespace CareConnect.Commands.Caregivers;

public sealed class UpdateCaregiverCommandValidator : AbstractValidator<UpdateCaregiverCommand>
{
    public UpdateCaregiverCommandValidator(IValidator<UpdateCaregiverDto> dtoValidator)
    {
        RuleFor(x => x.CaregiverId)
            .GreaterThan(0);

        RuleFor(x => x.Caregiver)
            .SetValidator(dtoValidator);
    }
}
