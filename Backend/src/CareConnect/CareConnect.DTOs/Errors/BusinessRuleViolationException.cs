namespace CareConnect.DTOs.Errors;

/// <summary>
/// The request is well-formed but conflicts with current server state (duplicate profile, wrong
/// role, etc.) — a business rule, not an input-shape problem. Maps to HTTP 409.
/// </summary>
public sealed class BusinessRuleViolationException : Exception
{
    public BusinessRuleViolationException(string message) : base(message)
    {
    }
}
