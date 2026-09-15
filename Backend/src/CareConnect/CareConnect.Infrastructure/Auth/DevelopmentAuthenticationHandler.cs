using System.Security.Claims;
using System.Text.Encodings.Web;
using CareConnect.DTOs.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CareConnect.Infrastructure.Auth;

/// <summary>
/// Stands in for real Auth0 JWT validation during local testing (Postman, etc.) when explicitly
/// enabled via Auth0:BypassForLocalTesting — gated in <c>InfrastructureServiceCollectionExtensions</c>
/// to require both this flag AND a Development environment, so it can never activate elsewhere.
///
/// Every request authenticates automatically. The caller picks who they are via two optional
/// headers instead of a token:
///   X-Dev-Sub  — the Auth0UserId to impersonate (must match a seeded Users row to pass the
///                app's row-level ownership checks). Defaults to "dev-admin".
///   X-Dev-Role — the role claim used by [Authorize(Roles = ...)]. Defaults to "Admin".
/// </summary>
public sealed class DevelopmentAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string DefaultSub = "dev-admin";
    private const string SubHeaderName = "X-Dev-Sub";
    private const string RoleHeaderName = "X-Dev-Role";

    public DevelopmentAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var sub = Request.Headers.TryGetValue(SubHeaderName, out var subValues) && !string.IsNullOrWhiteSpace(subValues)
            ? subValues.ToString()
            : DefaultSub;

        var role = Request.Headers.TryGetValue(RoleHeaderName, out var roleValues) && !string.IsNullOrWhiteSpace(roleValues)
            ? roleValues.ToString()
            : nameof(UserRole.Admin);

        var claims = new[]
        {
            new Claim("sub", sub),
            new Claim(ClaimTypes.Role, role),
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        Logger.LogWarning(
            "AUTH BYPASSED for local testing — authenticated as sub={Sub}, role={Role}. This must never run outside Development.",
            sub, role);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
