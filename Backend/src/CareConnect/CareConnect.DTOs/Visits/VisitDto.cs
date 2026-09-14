using CareConnect.DTOs.Enums;

namespace CareConnect.DTOs.Visits;

public sealed class VisitDto
{
    public required int Id { get; init; }

    public required int CaregiverAssignmentId { get; init; }

    public required string CaregiverFullName { get; init; }

    public required string ClientFullName { get; init; }

    public required DateTime ScheduledStartUtc { get; init; }

    public required DateTime ScheduledEndUtc { get; init; }

    public DateTime? ActualStartUtc { get; init; }

    public DateTime? ActualEndUtc { get; init; }

    public required VisitStatus Status { get; init; }

    public string? CancellationReason { get; init; }

    public required IReadOnlyList<VisitTaskDto> VisitTasks { get; init; }
}
