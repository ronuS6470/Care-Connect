namespace CareConnect.Infrastructure.Errors;

/// <summary>
/// The request collides with an existing resource (duplicate email, name already taken, an
/// overlapping window, a status that's already what the caller is trying to set it to) rather
/// than violating a workflow rule. Maps to HTTP 409. See also <see cref="BusinessRuleException"/>.
/// </summary>
public sealed class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}
