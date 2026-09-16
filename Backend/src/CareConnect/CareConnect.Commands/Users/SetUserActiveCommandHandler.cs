using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.Users;
using MediatR;

namespace CareConnect.Commands.Users;

public sealed class SetUserActiveCommandHandler : IRequestHandler<SetUserActiveCommand>
{
    private readonly IUserRepository _userRepository;

    public SetUserActiveCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(SetUserActiveCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {request.UserId} was not found.");

        if (user.IsActive == request.IsActive)
        {
            return;
        }

        // Same self-protection as a role change: you cannot disable your own account, so an Admin
        // can never lock the last administrator out of the system.
        if (!request.IsActive)
        {
            var caller = await _userRepository.GetByAuth0UserIdAsync(request.RequestingAuth0UserId, cancellationToken)
                ?? throw new ForbiddenException("This account is not recognized.");

            if (caller.Id == user.Id)
            {
                throw new BusinessRuleException("You cannot deactivate your own account.");
            }
        }

        user.IsActive = request.IsActive;
        user.UpdatedAtUtc = DateTime.UtcNow;

        // Deactivation blocks the next sign-in (see LoginCommandHandler); an access token already
        // issued keeps working until it expires.
        await _userRepository.SaveChangesAsync(cancellationToken);
    }
}
