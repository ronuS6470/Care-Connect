using System.Text.Json.Serialization;
using CareConnect.DTOs.Enums;

namespace CareConnect.DTOs.Auth;

public sealed class LoginResponseDto
{
    public required string Token { get; init; }

    public required int UserId { get; init; }

    public required string FullName { get; init; }

    public required string Email { get; init; }

    /// <summary>
    /// Serialized as its name ("Admin"/"Caregiver"/"Client"), not its underlying int, because the
    /// client's authentication contract is defined in terms of role names. The converter is applied
    /// to this property alone — the API has no global JsonStringEnumConverter, so every other enum
    /// on every other endpoint still crosses the wire as an integer, unchanged.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required UserRole Role { get; init; }

    public required DateTime ExpiresAtUtc { get; init; }
}
