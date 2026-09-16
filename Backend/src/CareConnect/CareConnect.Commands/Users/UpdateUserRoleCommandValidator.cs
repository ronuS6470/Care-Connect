using CareConnect.DTOs.Users;
using FluentValidation;

namespace CareConnect.Commands.Users;

public sealed class UpdateUserRoleCommandValidator : AbstractValidator<UpdateUserRoleCommand>
{
    public UpdateUserRoleCommandValidator(IValidator<UpdateUserRoleDto> dtoValidator)
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0);

        RuleFor(x => x.Role)
            .SetValidator(dtoValidator);
    }
}
