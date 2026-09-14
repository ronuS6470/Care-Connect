using CareConnect.DTOs.Visits;
using FluentValidation;

namespace CareConnect.Commands.Visits;

public sealed class UpdateVisitTaskCommandValidator : AbstractValidator<UpdateVisitTaskCommand>
{
    public UpdateVisitTaskCommandValidator(IValidator<UpdateVisitTaskDto> dtoValidator)
    {
        RuleFor(x => x.VisitId).GreaterThan(0);
        RuleFor(x => x.VisitTaskId).GreaterThan(0);
        RuleFor(x => x.RequestingAuth0UserId).NotEmpty();
        RuleFor(x => x.Task).SetValidator(dtoValidator);
    }
}
