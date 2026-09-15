using CareConnect.DTOs.Auth;
using CareConnect.Infrastructure.Auth;
using CareConnect.Infrastructure.Errors;
using CareConnect.Infrastructure.Repositories.Users;
using MediatR;

namespace CareConnect.Commands.Auth;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    /// <summary>One message for every failure mode — see InvalidCredentialsException.</summary>
    private const string RejectionMessage = "Invalid email or password.";

    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var credentials = request.Credentials;

        var user = await _userRepository.GetByEmailAsync(credentials.Email, cancellationToken);

        // No account, or an account with no password set (provisioned before local sign-in
        // existed). Burn an equivalent PBKDF2 round before refusing so response time doesn't
        // differ measurably from the wrong-password path and reveal which emails are registered.
        if (user?.PasswordHash is null)
        {
            _passwordHasher.Hash(credentials.Password);
            throw new InvalidCredentialsException(RejectionMessage);
        }

        if (!_passwordHasher.Verify(credentials.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException(RejectionMessage);
        }

        // Deactivated accounts keep their password but lose access. Same message deliberately:
        // the sign-in page is anonymous, so it is not the place to disclose account status.
        if (!user.IsActive)
        {
            throw new InvalidCredentialsException(RejectionMessage);
        }

        var issued = _tokenGenerator.Generate(user);

        return new LoginResponseDto
        {
            Token = issued.Token,
            UserId = user.Id,
            FullName = $"{user.FirstName} {user.LastName}",
            Email = user.Email,
            Role = user.Role,
            ExpiresAtUtc = issued.ExpiresAtUtc,
        };
    }
}
