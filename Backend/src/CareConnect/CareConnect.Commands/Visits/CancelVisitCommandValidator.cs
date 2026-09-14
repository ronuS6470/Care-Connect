using CareConnect.DTOs.Visits;
using FluentValidation;

namespace CareConnect.Commands.Visits;

public sealed class CancelVisitCommandValidator : AbstractValidator<CancelVisitCommand>
{
    public CancelVisitCommandValidator(IValidator<CancelVisitDto> dtoValidator)
    {
        RuleFor(x => x.VisitId)
            .GreaterThan(0);

        RuleFor(x => x.Cancellation)
            .SetValidator(dtoValidator);
    }
}
