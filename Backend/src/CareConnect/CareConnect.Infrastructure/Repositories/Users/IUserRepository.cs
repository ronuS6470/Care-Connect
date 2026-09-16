using CareConnect.Infrastructure.Entities;

namespace CareConnect.Infrastructure.Repositories.Users;

public interface IUserRepository
{
    /// <summary>Case-insensitive by virtue of the database collation; null when no such account exists.</summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>Resolves the caller from the "sub" claim value carried on every access token.</summary>
    Task<User?> GetByAuth0UserIdAsync(string auth0UserId, CancellationToken cancellationToken);

    /// <summary>True when this user already has the profile row their role implies.</summary>
    Task<bool> HasCaregiverProfileAsync(int userId, CancellationToken cancellationToken);

    Task<bool> HasClientProfileAsync(int userId, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
