namespace CareConnect.Commands.Auth;

/// <summary>Shared by every validator that accepts a new password, so the rule is stated once.</summary>
public static class PasswordPolicy
{
    public const int MinLength = 8;

    /// <summary>PBKDF2 has no practical input limit; this simply bounds the work an anonymous caller can ask for.</summary>
    public const int MaxLength = 128;
}
