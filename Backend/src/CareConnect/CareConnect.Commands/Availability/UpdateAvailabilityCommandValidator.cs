using CareConnect.DTOs.Caregivers;
using FluentValidation;

namespace CareConnect.Commands.Availability;

public sealed class UpdateAvailabilityCommandValidator : AbstractValidator<UpdateAvailabilityCommand>
{
    public UpdateAvailabilityCommandValidator(IValidator<UpdateCaregiverAvailabilityDto> dtoValidator)
    {
        RuleFor(x => x.AvailabilityId)
            .GreaterThan(0);

        RuleFor(x => x.Availability)
            .SetValidator(dtoValidator);
    }
}
