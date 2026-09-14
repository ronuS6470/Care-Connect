using CareConnect.DTOs.Assignments;
using FluentValidation;

namespace CareConnect.Commands.Validation.Assignments;

public class UpdateAssignmentDtoValidator : AbstractValidator<UpdateAssignmentDto>
{
    public UpdateAssignmentDtoValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum();

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .When(x => x.EndDate.HasValue);

        RuleFor(x => x.Notes)
            .MaximumLength(500);

        // EndDate >= the assignment's existing StartDate can't be checked here — StartDate isn't
        // part of this payload, only the target assignment's Id (from the route) is. That's a
        // business rule (needs the stored assignment), enforced by the command handler, not here.
    }
}
