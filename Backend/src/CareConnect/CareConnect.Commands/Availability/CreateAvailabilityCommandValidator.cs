using CareConnect.DTOs.Caregivers;
using FluentValidation;

namespace CareConnect.Commands.Availability;

public sealed class CreateAvailabilityCommandValidator : AbstractValidator<CreateAvailabilityCommand>
{
    public CreateAvailabilityCommandValidator(IValidator<CreateCaregiverAvailabilityDto> dtoValidator)
    {
        RuleFor(x => x.Availability).SetValidator(dtoValidator);
    }
}
