using CareConnect.DTOs.Caregivers;
using FluentValidation;

namespace CareConnect.Commands.Validation.Caregivers;

public class UpdateCaregiverAvailabilityDtoValidator : AbstractValidator<UpdateCaregiverAvailabilityDto>
{
    public UpdateCaregiverAvailabilityDtoValidator()
    {
        RuleFor(x => x.DayOfWeek)
            .IsInEnum();

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage("'{PropertyName}' must be after StartTime.");
    }
}
