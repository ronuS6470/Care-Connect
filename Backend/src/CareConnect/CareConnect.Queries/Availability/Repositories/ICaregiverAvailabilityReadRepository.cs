using CareConnect.DTOs.Caregivers;

namespace CareConnect.Queries.Availability.Repositories;

public interface ICaregiverAvailabilityReadRepository
{
    Task<IReadOnlyList<CaregiverAvailabilityDto>> GetByCaregiverIdAsync(int caregiverId, CancellationToken cancellationToken);
}
