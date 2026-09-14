namespace CareConnect.DTOs.Visits;

public sealed class VisitTaskDto
{
    public required int Id { get; init; }

    public required int VisitId { get; init; }

    public required int CareTaskId { get; init; }

    public required string CareTaskName { get; init; }

    public required bool IsCompleted { get; init; }

    public DateTime? CompletedAtUtc { get; init; }

    public string? Notes { get; init; }
}
