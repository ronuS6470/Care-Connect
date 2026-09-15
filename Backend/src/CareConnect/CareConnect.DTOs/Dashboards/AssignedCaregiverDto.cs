namespace CareConnect.DTOs.Dashboards;

public sealed class AssignedCaregiverDto
{
    public required int CaregiverId { get; init; }

    public required string FullName { get; init; }

    public string? PhoneNumber { get; init; }
}
