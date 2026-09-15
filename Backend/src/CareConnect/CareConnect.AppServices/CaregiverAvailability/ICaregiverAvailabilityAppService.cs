using CareConnect.DTOs.Caregivers;

namespace CareConnect.AppServices.CaregiverAvailability;

public interface ICaregiverAvailabilityAppService
{
    Task<IReadOnlyList<CaregiverAvailabilityDto>> GetCaregiverAvailabilityAsync(int caregiverId, CancellationToken cancellationToken);

    Task<int> CreateAvailabilityAsync(CreateCaregiverAvailabilityDto dto, CancellationToken cancellationToken);

    Task UpdateAvailabilityAsync(int availabilityId, UpdateCaregiverAvailabilityDto dto, CancellationToken cancellationToken);

    Task DeleteAvailabilityAsync(int availabilityId, CancellationToken cancellationToken);
}
