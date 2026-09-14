using CareConnect.DTOs.Visits;
using FluentValidation;

namespace CareConnect.Commands.Visits;

public sealed class CreateVisitCommandValidator : AbstractValidator<CreateVisitCommand>
{
    public CreateVisitCommandValidator(IValidator<CreateVisitDto> dtoValidator)
    {
        RuleFor(x => x.Visit).SetValidator(dtoValidator);
    }
}
