namespace CareConnect.Infrastructure.Errors;

/// <summary>
/// The request is well-formed but violates a domain workflow rule (wrong status for this
/// transition, outside an allowed window, etc.) rather than colliding with another resource.
/// Maps to HTTP 409. See also <see cref="ConflictException"/> for duplicate/already-exists
/// conflicts.
/// </summary>
public sealed class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message)
    {
    }
}
