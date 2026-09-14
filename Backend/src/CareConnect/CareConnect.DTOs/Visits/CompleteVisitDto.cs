namespace CareConnect.DTOs.Visits;

/// <summary>
/// Required only when some of the visit's tasks are still incomplete — documents why completion
/// is being allowed as an exception rather than blocked.
/// </summary>
public sealed class CompleteVisitDto
{
    public string? IncompleteTasksReason { get; init; }
}
