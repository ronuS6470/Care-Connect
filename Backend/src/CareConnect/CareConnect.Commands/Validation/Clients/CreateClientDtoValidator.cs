using CareConnect.DTOs.Clients;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence;
using FluentValidation;

namespace CareConnect.Commands.Validation.Clients;

public class CreateClientDtoValidator : AbstractValidator<CreateClientDto>
{
    public CreateClientDtoValidator(CareConnectDbContext dbContext)
    {
        RuleFor(x => x.UserId)
            .MustReferenceExisting<CreateClientDto, User>(dbContext, "user");

        RuleFor(x => x.AddressLine1)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.AddressLine2)
            .MaximumLength(200);

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.State)
            .NotEmpty()
            .Length(2)
            .WithMessage("'{PropertyName}' must be a 2-letter US state code.");

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .MustBeAValidUsZipCode();

        RuleFor(x => x.EmergencyContactName)
            .MaximumLength(150);

        RuleFor(x => x.EmergencyContactPhone)
            .MustBeAValidUsPhoneNumber()
            .When(x => !string.IsNullOrWhiteSpace(x.EmergencyContactPhone));
    }
}
