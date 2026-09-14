using FluentValidation;

namespace CareConnect.Commands.Clients;

public sealed class DeleteClientCommandValidator : AbstractValidator<DeleteClientCommand>
{
    public DeleteClientCommandValidator()
    {
        RuleFor(x => x.ClientId).GreaterThan(0);
    }
}
