using CareConnect.DTOs.Assignments;
using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;

namespace CareConnect.AppServices.Assignments;

public interface IAssignmentsAppService
{
    Task<PagedResponseDto<AssignmentDto>> GetAssignmentsAsync(
        int page, int pageSize, AssignmentStatus? status, CancellationToken cancellationToken);

    Task<AssignmentDto?> GetAssignmentByIdAsync(int assignmentId, CancellationToken cancellationToken);

    Task<int> CreateAssignmentAsync(CreateAssignmentDto dto, CancellationToken cancellationToken);

    Task UpdateAssignmentAsync(int assignmentId, UpdateAssignmentDto dto, CancellationToken cancellationToken);

    Task CancelAssignmentAsync(int assignmentId, CancellationToken cancellationToken);
}
