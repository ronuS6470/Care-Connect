using CareConnect.DTOs.CareTasks;
using FluentValidation;

namespace CareConnect.Commands.CareTasks;

/// <summary>Delegates to the existing CreateCareTaskDto validator rather than duplicating its rules.</summary>
public sealed class CreateCareTaskCommandValidator : AbstractValidator<CreateCareTaskCommand>
{
    public CreateCareTaskCommandValidator(IValidator<CreateCareTaskDto> dtoValidator)
    {
        RuleFor(x => x.CareTask).SetValidator(dtoValidator);
    }
}
