using CareConnect.DTOs.Users;
using FluentValidation;

namespace CareConnect.Commands.Validation.Users;

public class UpdateUserRoleDtoValidator : AbstractValidator<UpdateUserRoleDto>
{
    public UpdateUserRoleDtoValidator()
    {
        // Rejects values outside the enum (e.g. 0 or 99) before any handler runs.
        RuleFor(x => x.Role)
            .IsInEnum();
    }
}
