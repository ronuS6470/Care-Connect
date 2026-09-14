using CareConnect.DTOs.Visits;
using FluentValidation;

namespace CareConnect.Commands.Validation.Visits;

public class CancelVisitDtoValidator : AbstractValidator<CancelVisitDto>
{
    public CancelVisitDtoValidator()
    {
        RuleFor(x => x.CancellationReason)
            .MaximumLength(300);
    }
}
