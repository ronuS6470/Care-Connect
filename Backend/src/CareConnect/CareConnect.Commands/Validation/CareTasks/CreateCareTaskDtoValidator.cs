using CareConnect.DTOs.CareTasks;
using FluentValidation;

namespace CareConnect.Commands.Validation.CareTasks;

public class CreateCareTaskDtoValidator : AbstractValidator<CreateCareTaskDto>
{
    public CreateCareTaskDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}
