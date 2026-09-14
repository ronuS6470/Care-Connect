namespace CareConnect.DTOs.Visits;

/// <summary>
/// The author is the authenticated caller (from the JWT), never a client-supplied field —
/// otherwise a caller could write notes under someone else's name.
/// </summary>
public sealed class CreateVisitNoteDto
{
    public required int VisitId { get; init; }

    public required string Content { get; init; }
}
