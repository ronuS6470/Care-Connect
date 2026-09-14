using CareConnect.Infrastructure.Entities;

namespace CareConnect.Infrastructure.Repositories.Clients;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken);

    Task<bool> IsEmailUsedByAnotherUserAsync(int userId, string email, CancellationToken cancellationToken);

    Task<bool> ExistsForUserAsync(int userId, CancellationToken cancellationToken);

    void Add(Client client);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
