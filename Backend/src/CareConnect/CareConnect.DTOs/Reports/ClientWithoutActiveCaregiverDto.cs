namespace CareConnect.DTOs.Reports;

public sealed class ClientWithoutActiveCaregiverDto
{
    public required int ClientId { get; init; }

    public required string ClientFullName { get; init; }

    public required bool IsActive { get; init; }
}
