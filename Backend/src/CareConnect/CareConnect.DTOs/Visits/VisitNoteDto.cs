namespace CareConnect.DTOs.Visits;

public sealed class VisitNoteDto
{
    public required int Id { get; init; }

    public required int VisitId { get; init; }

    public required int AuthorUserId { get; init; }

    public required string AuthorFullName { get; init; }

    public required string Content { get; init; }

    public required DateTime CreatedAtUtc { get; init; }
}
