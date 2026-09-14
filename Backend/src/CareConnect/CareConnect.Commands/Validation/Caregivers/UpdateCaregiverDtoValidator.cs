using CareConnect.DTOs.Caregivers;
using FluentValidation;

namespace CareConnect.Commands.Validation.Caregivers;

public class UpdateCaregiverDtoValidator : AbstractValidator<UpdateCaregiverDto>
{
    public UpdateCaregiverDtoValidator()
    {
        RuleFor(x => x.LicenseNumber)
            .MaximumLength(50);

        RuleFor(x => x.HourlyRate)
            .GreaterThan(0m);

        RuleFor(x => x.HireDate)
            .NotEmpty();

        RuleFor(x => x.YearsOfExperience)
            .GreaterThanOrEqualTo(0);
    }
}
