using CareConnect.DTOs.Caregivers;
using CareConnect.DTOs.Common;

namespace CareConnect.Queries.Caregivers.Repositories;

public interface ICaregiverReadRepository
{
    Task<PagedResponseDto<CaregiverDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);

    Task<CaregiverDto?> GetByIdAsync(int caregiverId, CancellationToken cancellationToken);
}
