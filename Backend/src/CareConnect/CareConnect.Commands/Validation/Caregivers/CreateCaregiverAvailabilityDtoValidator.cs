using CareConnect.DTOs.Caregivers;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence;
using FluentValidation;

namespace CareConnect.Commands.Validation.Caregivers;

public class CreateCaregiverAvailabilityDtoValidator : AbstractValidator<CreateCaregiverAvailabilityDto>
{
    public CreateCaregiverAvailabilityDtoValidator(CareConnectDbContext dbContext)
    {
        RuleFor(x => x.CaregiverId)
            .MustReferenceExisting<CreateCaregiverAvailabilityDto, Caregiver>(dbContext, "caregiver");

        RuleFor(x => x.DayOfWeek)
            .IsInEnum();

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage("'{PropertyName}' must be after StartTime.");
    }
}
