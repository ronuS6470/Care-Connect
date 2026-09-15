using CareConnect.Infrastructure.Entities;

namespace CareConnect.Infrastructure.Repositories.Users;

public interface IUserRepository
{
    /// <summary>Case-insensitive by virtue of the database collation; null when no such account exists.</summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
