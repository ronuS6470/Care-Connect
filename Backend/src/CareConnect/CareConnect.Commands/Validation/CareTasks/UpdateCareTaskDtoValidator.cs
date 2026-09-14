using CareConnect.DTOs.CareTasks;
using FluentValidation;

namespace CareConnect.Commands.Validation.CareTasks;

public class UpdateCareTaskDtoValidator : AbstractValidator<UpdateCareTaskDto>
{
    public UpdateCareTaskDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}
