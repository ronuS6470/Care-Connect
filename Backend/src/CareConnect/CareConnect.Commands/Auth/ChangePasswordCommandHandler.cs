using CareConnect.Infrastructure.Auth;
using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.Users;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace CareConnect.Commands.Auth;

public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByAuth0UserIdAsync(request.RequestingAuth0UserId, cancellationToken)
            ?? throw new ForbiddenException("This account is not recognized.");

        // A wrong current password is reported as a field-level validation failure (400), not as a
        // 401 — the caller's *session* is perfectly valid, and a 401 here would read as "your login
        // expired" and sign them out mid-form.
        if (user.PasswordHash is null)
        {
            throw Invalid(nameof(request.Password.CurrentPassword), "This account has no password set. Ask an administrator to set one.");
        }

        if (!_passwordHasher.Verify(request.Password.CurrentPassword, user.PasswordHash))
        {
            throw Invalid(nameof(request.Password.CurrentPassword), "Current password is incorrect.");
        }

        if (_passwordHasher.Verify(request.Password.NewPassword, user.PasswordHash))
        {
            throw Invalid(nameof(request.Password.NewPassword), "New password must be different from the current one.");
        }

        user.PasswordHash = _passwordHasher.Hash(request.Password.NewPassword);
        user.UpdatedAtUtc = DateTime.UtcNow;

        // Access tokens already issued stay valid until they expire — this API has no token
        // revocation list, so a password change does not sign other sessions out.
        await _userRepository.SaveChangesAsync(cancellationToken);
    }

    private static ValidationException Invalid(string property, string message) =>
        new([new ValidationFailure(property, message)]);
}
