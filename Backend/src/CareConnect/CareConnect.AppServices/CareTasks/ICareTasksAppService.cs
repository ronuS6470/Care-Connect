using CareConnect.DTOs.CareTasks;
using CareConnect.DTOs.Common;

namespace CareConnect.AppServices.CareTasks;

public interface ICareTasksAppService
{
    Task<PagedResponseDto<CareTaskDto>> GetCareTasksAsync(int page, int pageSize, bool? isActive, CancellationToken cancellationToken);

    Task<CareTaskDto?> GetCareTaskByIdAsync(int careTaskId, CancellationToken cancellationToken);

    Task<int> CreateCareTaskAsync(CreateCareTaskDto dto, CancellationToken cancellationToken);

    Task UpdateCareTaskAsync(int careTaskId, UpdateCareTaskDto dto, CancellationToken cancellationToken);

    Task DeleteCareTaskAsync(int careTaskId, CancellationToken cancellationToken);
}
