using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.Users;
using MediatR;

namespace CareConnect.Commands.Users;

public sealed class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserRoleCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {request.UserId} was not found.");

        var newRole = request.Role.Role;

        if (user.Role == newRole)
        {
            return;
        }

        // Nobody may change their own role. That single rule also guarantees the system can never
        // be left without an Admin: an Admin can demote others, but never the account doing the
        // demoting, so at least one Admin always remains.
        var caller = await _userRepository.GetByAuth0UserIdAsync(request.RequestingAuth0UserId, cancellationToken)
            ?? throw new ForbiddenException("This account is not recognized.");

        if (caller.Id == user.Id)
        {
            throw new BusinessRuleException("You cannot change your own role. Ask another administrator to do it.");
        }

        // A Caregivers/Clients row is only ever created for a user already holding that role (see
        // CreateCaregiverCommandHandler). Moving the user off that role would strand the profile —
        // and everything joined to it — under an account that can no longer reach it.
        if (user.Role == UserRole.Caregiver && await _userRepository.HasCaregiverProfileAsync(user.Id, cancellationToken))
        {
            throw new BusinessRuleException(
                "This user still has a caregiver profile. Remove or reassign that profile before changing their role.");
        }

        if (user.Role == UserRole.Client && await _userRepository.HasClientProfileAsync(user.Id, cancellationToken))
        {
            throw new BusinessRuleException(
                "This user still has a client profile. Remove or reassign that profile before changing their role.");
        }

        user.Role = newRole;
        user.UpdatedAtUtc = DateTime.UtcNow;

        // Their current access token keeps the old role claim until it expires; the new role takes
        // effect the next time they sign in.
        await _userRepository.SaveChangesAsync(cancellationToken);
    }
}
