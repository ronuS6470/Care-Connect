using CareConnect.Infrastructure.Entities;

namespace CareConnect.Infrastructure.Repositories.Availability;

public interface ICaregiverAvailabilityRepository
{
    Task<CaregiverAvailability?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<bool> ExistsOverlappingWindowAsync(
        int caregiverId, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? excludingId, CancellationToken cancellationToken);

    void Add(CaregiverAvailability availability);

    void Remove(CaregiverAvailability availability);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
