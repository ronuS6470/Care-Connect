using CareConnect.DTOs.Assignments;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence;
using FluentValidation;

namespace CareConnect.Commands.Validation.Assignments;

public class CreateAssignmentDtoValidator : AbstractValidator<CreateAssignmentDto>
{
    public CreateAssignmentDtoValidator(CareConnectDbContext dbContext)
    {
        RuleFor(x => x.CaregiverId)
            .MustReferenceExisting<CreateAssignmentDto, Caregiver>(dbContext, "caregiver");

        RuleFor(x => x.ClientId)
            .MustReferenceExisting<CreateAssignmentDto, Client>(dbContext, "client");

        RuleFor(x => x.StartDate)
            .NotEmpty();

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.EndDate.HasValue)
            .WithMessage("'{PropertyName}' must be on or after StartDate.");

        RuleFor(x => x.Notes)
            .MaximumLength(500);
    }
}
