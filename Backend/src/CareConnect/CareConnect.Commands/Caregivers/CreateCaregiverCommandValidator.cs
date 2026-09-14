using CareConnect.DTOs.Caregivers;
using FluentValidation;

namespace CareConnect.Commands.Caregivers;

/// <summary>Delegates to the existing CreateCaregiverDto validator rather than duplicating its rules.</summary>
public sealed class CreateCaregiverCommandValidator : AbstractValidator<CreateCaregiverCommand>
{
    public CreateCaregiverCommandValidator(IValidator<CreateCaregiverDto> dtoValidator)
    {
        RuleFor(x => x.Caregiver).SetValidator(dtoValidator);
    }
}
