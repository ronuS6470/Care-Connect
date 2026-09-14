namespace CareConnect.Infrastructure.Errors;

/// <summary>
/// The caller is authenticated but not allowed to access this specific resource (e.g. a Client
/// user requesting another client's record). Maps to HTTP 403.
/// </summary>
public sealed class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message)
    {
    }
}
