using CareConnect.DTOs.Visits;
using FluentValidation;

namespace CareConnect.Commands.Visits;

public sealed class UpdateVisitCommandValidator : AbstractValidator<UpdateVisitCommand>
{
    public UpdateVisitCommandValidator(IValidator<UpdateVisitDto> dtoValidator)
    {
        RuleFor(x => x.VisitId)
            .GreaterThan(0);

        RuleFor(x => x.Visit)
            .SetValidator(dtoValidator);
    }
}
