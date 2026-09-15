using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Infrastructure.Repositories.CareTasks;

public sealed class CareTaskRepository : ICareTaskRepository
{
    private readonly CareConnectDbContext _dbContext;

    public CareTaskRepository(CareConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<CareTask?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        _dbContext.CareTasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public Task<bool> ExistsWithNameAsync(string name, int? excludingId, CancellationToken cancellationToken) =>
        _dbContext.CareTasks.AnyAsync(t => t.Name == name && t.Id != excludingId, cancellationToken);

    public Task<bool> IsReferencedByVisitTaskAsync(int careTaskId, CancellationToken cancellationToken) =>
        _dbContext.VisitTasks.AnyAsync(vt => vt.CareTaskId == careTaskId, cancellationToken);

    public void Add(CareTask careTask) => _dbContext.CareTasks.Add(careTask);

    public void Remove(CareTask careTask) => _dbContext.CareTasks.Remove(careTask);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}
