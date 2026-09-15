using System.Data;
using CareConnect.Infrastructure.Errors;
using CareConnect.Queries.Common;
using Dapper;

namespace CareConnect.Queries.Security;

/// <summary>
/// Resolves "who is actually calling" from the DB, so authorization decisions are based on real
/// relationships (User → Client / User → Caregiver → CaregiverAssignment), never on a URL Id
/// taken at face value.
/// </summary>
public static class RequesterResolver
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(RequesterResolver), "ResolveRequesterQuery.sql");

    public static async Task<RequesterContext> ResolveAsync(
        IDbConnection connection,
        string auth0UserId,
        CancellationToken cancellationToken)
    {
        var requester = await connection.QuerySingleOrDefaultAsync<RequesterContext>(new CommandDefinition(
            Sql,
            new { Auth0UserId = auth0UserId },
            cancellationToken: cancellationToken));

        return requester ?? throw new ForbiddenException("This account is not recognized by CareConnect.");
    }
}
