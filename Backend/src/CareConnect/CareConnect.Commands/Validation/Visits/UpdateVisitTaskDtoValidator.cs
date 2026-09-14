using CareConnect.DTOs.Visits;
using FluentValidation;

namespace CareConnect.Commands.Validation.Visits;

public class UpdateVisitTaskDtoValidator : AbstractValidator<UpdateVisitTaskDto>
{
    public UpdateVisitTaskDtoValidator()
    {
        RuleFor(x => x.Notes)
            .MaximumLength(300);
    }
}
