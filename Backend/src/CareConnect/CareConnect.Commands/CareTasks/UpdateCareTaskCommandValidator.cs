using CareConnect.DTOs.CareTasks;
using FluentValidation;

namespace CareConnect.Commands.CareTasks;

public sealed class UpdateCareTaskCommandValidator : AbstractValidator<UpdateCareTaskCommand>
{
    public UpdateCareTaskCommandValidator(IValidator<UpdateCareTaskDto> dtoValidator)
    {
        RuleFor(x => x.CareTaskId)
            .GreaterThan(0);

        RuleFor(x => x.CareTask)
            .SetValidator(dtoValidator);
    }
}
