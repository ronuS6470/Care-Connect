namespace CareConnect.Commands.Validation;

/// <summary>
/// Single source of truth for turning user-supplied email text into the canonical form stored
/// and compared throughout the system, so "Jane@Example.com " and "jane@example.com" are the
/// same account. Validators check the normalized value; command handlers must normalize the
/// same way before persisting.
/// </summary>
public static class EmailNormalizer
{
    public static string Normalize(string email) => email.Trim().ToLowerInvariant();
}
