using CareConnect.DTOs.Visits;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence;
using FluentValidation;

namespace CareConnect.Commands.Validation.Visits;

public class CreateVisitNoteDtoValidator : AbstractValidator<CreateVisitNoteDto>
{
    public CreateVisitNoteDtoValidator(CareConnectDbContext dbContext)
    {
        RuleFor(x => x.VisitId)
            .MustReferenceExisting<CreateVisitNoteDto, Visit>(dbContext, "visit");

        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
