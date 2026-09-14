using FluentValidation;

namespace CareConnect.Commands.Visits;

public sealed class CheckInVisitCommandValidator : AbstractValidator<CheckInVisitCommand>
{
    public CheckInVisitCommandValidator()
    {
        RuleFor(x => x.VisitId).GreaterThan(0);
        RuleFor(x => x.RequestingAuth0UserId).NotEmpty();
    }
}
