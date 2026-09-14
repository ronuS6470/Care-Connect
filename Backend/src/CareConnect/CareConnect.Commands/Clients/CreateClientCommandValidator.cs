using CareConnect.DTOs.Clients;
using FluentValidation;

namespace CareConnect.Commands.Clients;

/// <summary>Delegates to the existing CreateClientDto validator rather than duplicating its rules.</summary>
public sealed class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator(IValidator<CreateClientDto> dtoValidator)
    {
        RuleFor(x => x.Client).SetValidator(dtoValidator);
    }
}
