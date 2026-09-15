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
}
