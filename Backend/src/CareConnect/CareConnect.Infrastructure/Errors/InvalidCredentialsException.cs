namespace CareConnect.Infrastructure.Errors;

/// <summary>
/// Sign-in was refused: no such account, wrong password, the account is deactivated, or it has no
/// password set. Maps to HTTP 401.
///
/// Callers are deliberately given one indistinguishable message for every one of those cases —
/// telling an anonymous caller which part was wrong turns the login form into an account
/// enumeration oracle.
/// </summary>
public sealed class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException(string message) : base(message)
    {
    }
}
