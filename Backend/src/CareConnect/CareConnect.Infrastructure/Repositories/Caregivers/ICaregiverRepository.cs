using CareConnect.Infrastructure.Entities;

namespace CareConnect.Infrastructure.Repositories.Caregivers;

public interface ICaregiverRepository
{
    Task<Caregiver?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken);

    Task<bool> IsEmailUsedByAnotherUserAsync(int userId, string email, CancellationToken cancellationToken);

    Task<bool> ExistsForUserAsync(int userId, CancellationToken cancellationToken);

    void Add(Caregiver caregiver);

    Task SetActiveStatusAsync(int caregiverId, bool isActive, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
