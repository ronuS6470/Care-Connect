namespace CareConnect.DTOs.Clients;

public sealed class UpdateClientDto
{
    public required string AddressLine1 { get; init; }

    public string? AddressLine2 { get; init; }

    public required string City { get; init; }

    public required string State { get; init; }

    public required string PostalCode { get; init; }

    public string? EmergencyContactName { get; init; }

    public string? EmergencyContactPhone { get; init; }

    public required bool IsActive { get; init; }
}
