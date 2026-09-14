using FluentValidation;

namespace CareConnect.Commands.Assignments;

public sealed class CancelAssignmentCommandValidator : AbstractValidator<CancelAssignmentCommand>
{
    public CancelAssignmentCommandValidator()
    {
        RuleFor(x => x.AssignmentId).GreaterThan(0);
    }
}
