using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Infrastructure.Repositories.Users;

public sealed class UserRepository : IUserRepository
{
    private readonly CareConnectDbContext _dbContext;

    public UserRepository(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<User?> GetByAuth0UserIdAsync(string auth0UserId, CancellationToken cancellationToken) =>
        _dbContext.Users.FirstOrDefaultAsync(u => u.Auth0UserId == auth0UserId, cancellationToken);

    public Task<bool> HasCaregiverProfileAsync(int userId, CancellationToken cancellationToken) =>
        _dbContext.Caregivers.AnyAsync(c => c.UserId == userId, cancellationToken);

    public Task<bool> HasClientProfileAsync(int userId, CancellationToken cancellationToken) =>
        _dbContext.Clients.AnyAsync(c => c.UserId == userId, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}
