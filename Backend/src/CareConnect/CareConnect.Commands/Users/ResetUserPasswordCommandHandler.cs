using CareConnect.Infrastructure.Auth;
using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.Users;
using MediatR;

namespace CareConnect.Commands.Users;

public sealed class ResetUserPasswordCommandHandler : IRequestHandler<ResetUserPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public ResetUserPasswordCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {request.UserId} was not found.");

        user.PasswordHash = _passwordHasher.Hash(request.Password.NewPassword);
        user.UpdatedAtUtc = DateTime.UtcNow;

        // As with a self-service change, any access token already issued to this user keeps working
        // until it expires — resetting a password does not end their current session.
        await _userRepository.SaveChangesAsync(cancellationToken);
    }
}
