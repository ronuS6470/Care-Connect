using System.Security.Cryptography;

namespace CareConnect.Infrastructure.Auth;

public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string storedHash);
}

/// <summary>
/// PBKDF2-HMAC-SHA256, using the framework's own KDF primitive — no hand-rolled cryptography and
/// no extra package (Microsoft.Extensions.Identity.Core is not referenced by this project).
///
/// Stored format: <c>v1.{iterations}.{base64Salt}.{base64Hash}</c>. The iteration count travels
/// with each hash rather than being read from a constant at verify time, so <see cref="Iterations"/>
/// can be raised later without invalidating passwords already stored at a lower count.
/// </summary>
public sealed class PasswordHasher : IPasswordHasher
{
    private const int Iterations = 210_000;
    private const int SaltSizeBytes = 16;
    private const int HashSizeBytes = 32;
    private const string Prefix = "v1";

    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSizeBytes);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSizeBytes);

        return $"{Prefix}.{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    /// <summary>
    /// Returns false — never throws — for a malformed or unrecognised stored hash, so a corrupt
    /// row fails the sign-in like any wrong password instead of surfacing as a 500.
    /// </summary>
    public bool Verify(string password, string storedHash)
    {
        var parts = storedHash.Split('.');

        if (parts.Length != 4 || parts[0] != Prefix || !int.TryParse(parts[1], out var iterations) || iterations <= 0)
        {
            return false;
        }

        byte[] salt;
        byte[] expectedHash;

        try
        {
            salt = Convert.FromBase64String(parts[2]);
            expectedHash = Convert.FromBase64String(parts[3]);
        }
        catch (FormatException)
        {
            return false;
        }

        var actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, Algorithm, expectedHash.Length);

        // Constant-time: a byte-by-byte early exit would leak how much of the digest matched.
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}
