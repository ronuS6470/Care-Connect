using CareConnect.DTOs.Caregivers;
using CareConnect.DTOs.Common;

namespace CareConnect.AppServices.Caregivers;

public interface ICaregiversAppService
{
    Task<PagedResponseDto<CaregiverDto>> GetCaregiversAsync(int page, int pageSize, CancellationToken cancellationToken);

    Task<CaregiverDto?> GetCaregiverByIdAsync(int caregiverId, CancellationToken cancellationToken);

    Task<int> CreateCaregiverAsync(CreateCaregiverDto dto, CancellationToken cancellationToken);

    Task UpdateCaregiverAsync(int caregiverId, UpdateCaregiverDto dto, CancellationToken cancellationToken);

    Task DeleteCaregiverAsync(int caregiverId, CancellationToken cancellationToken);

    Task ActivateCaregiverAsync(int caregiverId, CancellationToken cancellationToken);

    Task DeactivateCaregiverAsync(int caregiverId, CancellationToken cancellationToken);
}
