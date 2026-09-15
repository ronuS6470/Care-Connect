using CareConnect.Infrastructure.Entities;

namespace CareConnect.Infrastructure.Repositories.Assignments;

public interface IAssignmentRepository
{
    Task<CaregiverAssignment?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<bool> ExistsActiveAssignmentAsync(int caregiverId, int clientId, CancellationToken cancellationToken);

    void Add(CaregiverAssignment assignment);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
