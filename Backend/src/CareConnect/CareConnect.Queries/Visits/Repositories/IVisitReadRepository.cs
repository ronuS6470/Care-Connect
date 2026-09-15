using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Reporting;
using CareConnect.DTOs.Visits;

namespace CareConnect.Queries.Visits.Repositories;

public interface IVisitReadRepository
{
    Task<PagedResponseDto<VisitSummaryDto>> GetPagedAsync(
        int page,
        int pageSize,
        DateOnly? fromDate,
        DateOnly? toDate,
        int? caregiverId,
        int? clientId,
        VisitStatus? status,
        string? search,
        string requestingAuth0UserId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Distinct from a page-1 GetPagedAsync call filtered to today: "upcoming" needs a precise
    /// ScheduledStartUtc &gt;= now comparison (excluding visits earlier today that already
    /// started), not a date-only comparison — so this owns its own query.
    /// </summary>
    Task<PagedResponseDto<VisitSummaryDto>> GetUpcomingAsync(
        int page, int pageSize, string requestingAuth0UserId, CancellationToken cancellationToken);

    Task<VisitDto?> GetByIdAsync(int visitId, string requestingAuth0UserId, CancellationToken cancellationToken);

    Task<IReadOnlyList<VisitTaskDto>> GetTasksAsync(int visitId, string requestingAuth0UserId, CancellationToken cancellationToken);

    Task<IReadOnlyList<VisitNoteDto>> GetNotesAsync(int visitId, string requestingAuth0UserId, CancellationToken cancellationToken);
}
