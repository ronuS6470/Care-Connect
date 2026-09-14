using CareConnect.DTOs.Visits;
using FluentValidation;

namespace CareConnect.Commands.Visits;

public sealed class CompleteVisitCommandValidator : AbstractValidator<CompleteVisitCommand>
{
    public CompleteVisitCommandValidator(IValidator<CompleteVisitDto> dtoValidator)
    {
        RuleFor(x => x.VisitId).GreaterThan(0);
        RuleFor(x => x.RequestingAuth0UserId).NotEmpty();
        RuleFor(x => x.Completion).SetValidator(dtoValidator);
    }
}
