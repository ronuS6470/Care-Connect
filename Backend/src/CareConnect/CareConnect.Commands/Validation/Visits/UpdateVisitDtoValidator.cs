using CareConnect.DTOs.Visits;
using FluentValidation;

namespace CareConnect.Commands.Validation.Visits;

public class UpdateVisitDtoValidator : AbstractValidator<UpdateVisitDto>
{
    public UpdateVisitDtoValidator()
    {
        RuleFor(x => x.ScheduledStartUtc)
            .NotEmpty()
            .Must(startUtc => startUtc >= DateTime.UtcNow)
            .WithMessage("'{PropertyName}' cannot be in the past.");

        RuleFor(x => x.ScheduledEndUtc)
            .GreaterThan(x => x.ScheduledStartUtc)
            .WithMessage("'{PropertyName}' must be after ScheduledStartUtc.");
    }
}
