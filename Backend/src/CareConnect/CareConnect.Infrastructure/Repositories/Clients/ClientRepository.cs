using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Infrastructure.Repositories.Clients;

public sealed class ClientRepository : IClientRepository
{
    private readonly CareConnectDbContext _dbContext;

    public ClientRepository(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Client?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        _dbContext.Clients.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken) =>
        _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

    public Task<bool> IsEmailUsedByAnotherUserAsync(int userId, string email, CancellationToken cancellationToken) =>
        _dbContext.Users.AnyAsync(u => u.Id != userId && u.Email == email, cancellationToken);

    public Task<bool> ExistsForUserAsync(int userId, CancellationToken cancellationToken) =>
        _dbContext.Clients.AnyAsync(c => c.UserId == userId, cancellationToken);

    public void Add(Client client) => _dbContext.Clients.Add(client);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}
