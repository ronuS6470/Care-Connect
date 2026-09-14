namespace CareConnect.DTOs.Clients;

/// <summary>Attaches a client profile to an existing user (created via registration/Auth0 sync).</summary>
public sealed class CreateClientDto
{
    public required int UserId { get; init; }

    public required string AddressLine1 { get; init; }

    public string? AddressLine2 { get; init; }

    public required string City { get; init; }

    public required string State { get; init; }

    public required string PostalCode { get; init; }

    public string? EmergencyContactName { get; init; }

    public string? EmergencyContactPhone { get; init; }
}
