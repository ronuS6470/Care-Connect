using CareConnect.DTOs.Clients;
using FluentValidation;

namespace CareConnect.Commands.Clients;

public sealed class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
{
    public UpdateClientCommandValidator(IValidator<UpdateClientDto> dtoValidator)
    {
        RuleFor(x => x.ClientId)
            .GreaterThan(0);

        RuleFor(x => x.Client)
            .SetValidator(dtoValidator);
    }
}
