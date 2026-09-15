using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Entities.Common;

namespace CareConnect.Infrastructure.Entities;

public class User : IAuditable
{
    public int Id { get; set; }

    /// <summary>
    /// The stable subject identifier put in the "sub" claim of every issued token, and the value
    /// every handler resolves the caller by. Named for the original Auth0 integration; it remains
    /// the subject id now that this API issues its own JWTs.
    /// </summary>
    public string Auth0UserId { get; set; } = null!;

    public string Email { get; set; } = null!;

    /// <summary>
    /// PBKDF2 digest produced by <c>CareConnect.Infrastructure.Auth.PasswordHasher</c> — never a
    /// plaintext password. Null for accounts provisioned before local password sign-in existed
    /// (externally-identified users); those simply cannot sign in with a password.
    /// </summary>
    public string? PasswordHash { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public UserRole Role { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public Caregiver? Caregiver { get; set; }

    public Client? Client { get; set; }
}
