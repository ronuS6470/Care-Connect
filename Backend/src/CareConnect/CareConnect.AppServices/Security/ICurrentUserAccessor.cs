namespace CareConnect.AppServices.Security;

/// <summary>The Auth0 subject id of the currently authenticated caller, resolved from the request's JWT.</summary>
public interface ICurrentUserAccessor
{
    string Auth0UserId { get; }
}
