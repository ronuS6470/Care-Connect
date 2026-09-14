using CareConnect.DTOs.Assignments;
using FluentValidation;

namespace CareConnect.Commands.Assignments;

public sealed class CreateAssignmentCommandValidator : AbstractValidator<CreateAssignmentCommand>
{
    public CreateAssignmentCommandValidator(IValidator<CreateAssignmentDto> dtoValidator)
    {
        RuleFor(x => x.Assignment).SetValidator(dtoValidator);
    }
}
