using CareConnect.DTOs.CareTasks;
using CareConnect.DTOs.Common;

namespace CareConnect.Queries.CareTasks.Repositories;

public interface ICareTaskReadRepository
{
    Task<PagedResponseDto<CareTaskDto>> GetPagedAsync(int page, int pageSize, bool? isActive, CancellationToken cancellationToken);

    Task<CareTaskDto?> GetByIdAsync(int careTaskId, CancellationToken cancellationToken);
}
