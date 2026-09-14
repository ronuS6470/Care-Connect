using FluentValidation;

namespace CareConnect.Commands.CareTasks;

public sealed class DeleteCareTaskCommandValidator : AbstractValidator<DeleteCareTaskCommand>
{
    public DeleteCareTaskCommandValidator()
    {
        RuleFor(x => x.CareTaskId).GreaterThan(0);
    }
}
