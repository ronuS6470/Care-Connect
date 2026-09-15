using CareConnect.DTOs.Enums;

namespace CareConnect.Queries.Visits.GetVisitById;

/// <summary>
/// Row shape for GetVisitByIdQuery.sql. Carries CaregiverId/ClientId, which VisitDto does not, so
/// the handler can check ownership before building the DTO.
/// </summary>
internal sealed class VisitRow
{
    public int Id { get; init; }

    public int CaregiverAssignmentId { get; init; }

    public int CaregiverId { get; init; }

    public int ClientId { get; init; }

    public string CaregiverFullName { get; init; } = string.Empty;

    public string ClientFullName { get; init; } = string.Empty;

    public DateTime ScheduledStartUtc { get; init; }

    public DateTime ScheduledEndUtc { get; init; }

    public DateTime? ActualStartUtc { get; init; }

    public DateTime? ActualEndUtc { get; init; }

    public VisitStatus Status { get; init; }

    public string? CancellationReason { get; init; }
}
