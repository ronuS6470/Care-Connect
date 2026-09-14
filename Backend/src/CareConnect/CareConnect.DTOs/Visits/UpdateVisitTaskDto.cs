namespace CareConnect.DTOs.Visits;

/// <summary>
/// Notes only — completion is a status transition, owned by CompleteVisitTaskCommand /
/// UncompleteVisitTaskCommand, never a generic update field.
/// </summary>
public sealed class UpdateVisitTaskDto
{
    public string? Notes { get; init; }
}
