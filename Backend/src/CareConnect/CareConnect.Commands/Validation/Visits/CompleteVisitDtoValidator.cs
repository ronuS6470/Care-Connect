using CareConnect.DTOs.Visits;
using FluentValidation;

namespace CareConnect.Commands.Validation.Visits;

public class CompleteVisitDtoValidator : AbstractValidator<CompleteVisitDto>
{
    public CompleteVisitDtoValidator()
    {
        RuleFor(x => x.IncompleteTasksReason)
            .MaximumLength(500);
    }
}
