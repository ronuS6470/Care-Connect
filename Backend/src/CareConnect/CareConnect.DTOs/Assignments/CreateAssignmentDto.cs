namespace CareConnect.DTOs.Assignments;

public sealed class CreateAssignmentDto
{
    public required int CaregiverId { get; init; }

    public required int ClientId { get; init; }

    public required DateOnly StartDate { get; init; }

    public DateOnly? EndDate { get; init; }

    public string? Notes { get; init; }
}
