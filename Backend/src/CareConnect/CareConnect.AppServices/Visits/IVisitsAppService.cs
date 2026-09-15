using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reporting;
using CareConnect.DTOs.Visits;

namespace CareConnect.AppServices.Visits;

public interface IVisitsAppService
{
    Task<PagedResponseDto<VisitSummaryDto>> GetVisitsAsync(
        int page, int pageSize, DateOnly? fromDate, DateOnly? toDate, int? caregiverId, int? clientId,
        VisitStatus? status, string? search, CancellationToken cancellationToken);

    Task<IReadOnlyList<VisitSummaryDto>> GetTodaysVisitsAsync(CancellationToken cancellationToken);

    Task<PagedResponseDto<VisitSummaryDto>> GetUpcomingVisitsAsync(int page, int pageSize, CancellationToken cancellationToken);

    Task<VisitDto?> GetVisitByIdAsync(int visitId, CancellationToken cancellationToken);

    Task<int> CreateVisitAsync(CreateVisitDto dto, CancellationToken cancellationToken);

    Task UpdateVisitAsync(int visitId, UpdateVisitDto dto, CancellationToken cancellationToken);

    Task CancelVisitAsync(int visitId, CancelVisitDto dto, CancellationToken cancellationToken);

    Task CheckInAsync(int visitId, CancellationToken cancellationToken);

    Task CheckOutAsync(int visitId, CancellationToken cancellationToken);

    Task CompleteAsync(int visitId, CompleteVisitDto dto, CancellationToken cancellationToken);

    Task<IReadOnlyList<VisitTaskDto>> GetVisitTasksAsync(int visitId, CancellationToken cancellationToken);

    Task UpdateVisitTaskAsync(int visitId, int taskId, UpdateVisitTaskDto dto, CancellationToken cancellationToken);

    Task CompleteVisitTaskAsync(int visitId, int taskId, CancellationToken cancellationToken);

    Task UncompleteVisitTaskAsync(int visitId, int taskId, CancellationToken cancellationToken);
}
