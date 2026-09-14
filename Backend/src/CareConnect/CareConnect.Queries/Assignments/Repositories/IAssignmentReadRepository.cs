using CareConnect.DTOs.Assignments;
using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;

namespace CareConnect.Queries.Assignments.Repositories;

public interface IAssignmentReadRepository
{
    Task<PagedResponseDto<AssignmentDto>> GetPagedAsync(
        int page, int pageSize, AssignmentStatus? status, string requestingAuth0UserId, CancellationToken cancellationToken);

    Task<AssignmentDto?> GetByIdAsync(int assignmentId, string requestingAuth0UserId, CancellationToken cancellationToken);
}
