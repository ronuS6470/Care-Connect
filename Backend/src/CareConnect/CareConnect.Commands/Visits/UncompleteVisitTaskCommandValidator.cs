using FluentValidation;

namespace CareConnect.Commands.Visits;

public sealed class UncompleteVisitTaskCommandValidator : AbstractValidator<UncompleteVisitTaskCommand>
{
    public UncompleteVisitTaskCommandValidator()
    {
        RuleFor(x => x.VisitId).GreaterThan(0);
        RuleFor(x => x.VisitTaskId).GreaterThan(0);
        RuleFor(x => x.RequestingAuth0UserId).NotEmpty();
    }
}
