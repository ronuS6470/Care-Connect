using CareConnect.DTOs.Visits;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.Validation.Visits;

public class CreateVisitDtoValidator : AbstractValidator<CreateVisitDto>
{
    public CreateVisitDtoValidator(CareConnectDbContext dbContext)
    {
        RuleFor(x => x.CaregiverAssignmentId)
            .MustReferenceExisting<CreateVisitDto, CaregiverAssignment>(dbContext, "caregiver assignment");

        RuleFor(x => x.ScheduledStartUtc)
            .NotEmpty()
            .Must(startUtc => startUtc >= DateTime.UtcNow)
            .WithMessage("'{PropertyName}' cannot be in the past.");

        RuleFor(x => x.ScheduledEndUtc)
            .GreaterThan(x => x.ScheduledStartUtc)
            .WithMessage("'{PropertyName}' must be after ScheduledStartUtc.");

        RuleFor(x => x.CareTaskIds)
            .NotEmpty()
            .WithMessage("At least one care task must be selected.");

        RuleForEach(x => x.CareTaskIds)
            .MustAsync(async (id, cancellationToken) =>
                await dbContext.Set<CareTask>().AnyAsync(t => t.Id == id, cancellationToken))
            .WithMessage("CareTaskIds contains an id that does not refer to an existing care task.");
    }
}
