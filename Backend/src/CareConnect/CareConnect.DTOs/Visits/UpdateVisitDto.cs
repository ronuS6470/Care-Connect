namespace CareConnect.DTOs.Visits;

/// <summary>
/// Reschedule only — Status/ActualStart/ActualEnd/CancellationReason are deliberately absent.
/// Status transitions go through dedicated commands (e.g. CancelVisitCommand), never a generic
/// update, so a client can never smuggle an arbitrary status change through this endpoint.
/// </summary>
public sealed class UpdateVisitDto
{
    public required DateTime ScheduledStartUtc { get; init; }

    public required DateTime ScheduledEndUtc { get; init; }
}
