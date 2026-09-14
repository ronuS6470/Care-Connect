using CareConnect.DTOs.Visits;
using FluentValidation;

namespace CareConnect.Commands.Visits;

public sealed class AddVisitNoteCommandValidator : AbstractValidator<AddVisitNoteCommand>
{
    public AddVisitNoteCommandValidator(IValidator<CreateVisitNoteDto> dtoValidator)
    {
        RuleFor(x => x.RequestingAuth0UserId).NotEmpty();
        RuleFor(x => x.Note).SetValidator(dtoValidator);
    }
}
