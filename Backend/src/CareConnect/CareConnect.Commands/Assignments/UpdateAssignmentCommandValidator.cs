using CareConnect.DTOs.Assignments;
using FluentValidation;

namespace CareConnect.Commands.Assignments;

public sealed class UpdateAssignmentCommandValidator : AbstractValidator<UpdateAssignmentCommand>
{
    public UpdateAssignmentCommandValidator(IValidator<UpdateAssignmentDto> dtoValidator)
    {
        RuleFor(x => x.AssignmentId)
            .GreaterThan(0);

        RuleFor(x => x.Assignment)
            .SetValidator(dtoValidator);
    }
}
