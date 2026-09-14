using CareConnect.DTOs.Enums;

namespace CareConnect.DTOs.Assignments;

public sealed class UpdateAssignmentDto
{
    public required AssignmentStatus Status { get; init; }

    public DateOnly? EndDate { get; init; }

    public string? Notes { get; init; }
}
