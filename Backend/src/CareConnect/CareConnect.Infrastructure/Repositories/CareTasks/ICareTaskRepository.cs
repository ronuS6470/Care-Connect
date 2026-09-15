using CareConnect.Infrastructure.Entities;

namespace CareConnect.Infrastructure.Repositories.CareTasks;

public interface ICareTaskRepository
{
    Task<CareTask?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<bool> ExistsWithNameAsync(string name, int? excludingId, CancellationToken cancellationToken);

    Task<bool> IsReferencedByVisitTaskAsync(int careTaskId, CancellationToken cancellationToken);

    void Add(CareTask careTask);

    void Remove(CareTask careTask);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
