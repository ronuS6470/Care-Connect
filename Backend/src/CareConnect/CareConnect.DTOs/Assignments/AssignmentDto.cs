using CareConnect.DTOs.Enums;

namespace CareConnect.DTOs.Assignments;

public sealed class AssignmentDto
{
    public required int Id { get; init; }

    public required int CaregiverId { get; init; }

    public required string CaregiverFullName { get; init; }

    public required int ClientId { get; init; }

    public required string ClientFullName { get; init; }

    public required AssignmentStatus Status { get; init; }

    public required DateOnly StartDate { get; init; }

    public DateOnly? EndDate { get; init; }

    public string? Notes { get; init; }
}
