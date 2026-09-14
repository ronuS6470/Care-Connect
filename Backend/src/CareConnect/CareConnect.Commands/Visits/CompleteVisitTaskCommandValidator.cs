using FluentValidation;

namespace CareConnect.Commands.Visits;

public sealed class CompleteVisitTaskCommandValidator : AbstractValidator<CompleteVisitTaskCommand>
{
    public CompleteVisitTaskCommandValidator()
    {
        RuleFor(x => x.VisitId).GreaterThan(0);
        RuleFor(x => x.VisitTaskId).GreaterThan(0);
        RuleFor(x => x.RequestingAuth0UserId).NotEmpty();
    }
}
