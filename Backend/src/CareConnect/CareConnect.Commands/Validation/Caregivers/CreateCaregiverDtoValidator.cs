using CareConnect.DTOs.Caregivers;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence;
using FluentValidation;

namespace CareConnect.Commands.Validation.Caregivers;

public class CreateCaregiverDtoValidator : AbstractValidator<CreateCaregiverDto>
{
    public CreateCaregiverDtoValidator(CareConnectDbContext dbContext)
    {
        RuleFor(x => x.UserId)
            .MustReferenceExisting<CreateCaregiverDto, User>(dbContext, "user");

        RuleFor(x => x.LicenseNumber)
            .MaximumLength(50);

        RuleFor(x => x.HourlyRate)
            .GreaterThan(0m);

        RuleFor(x => x.HireDate)
            .NotEmpty();

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .Must(dob => dob <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("'{PropertyName}' cannot be in the future.")
            .Must(dob => dob <= DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-18)))
            .WithMessage("Caregiver must be at least 18 years old.");

        RuleFor(x => x.YearsOfExperience)
            .GreaterThanOrEqualTo(0);
    }
}
