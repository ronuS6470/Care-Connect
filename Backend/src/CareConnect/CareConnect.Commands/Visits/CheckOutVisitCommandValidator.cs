using FluentValidation;

namespace CareConnect.Commands.Visits;

public sealed class CheckOutVisitCommandValidator : AbstractValidator<CheckOutVisitCommand>
{
    public CheckOutVisitCommandValidator()
    {
        RuleFor(x => x.VisitId).GreaterThan(0);
        RuleFor(x => x.RequestingAuth0UserId).NotEmpty();
    }
}
