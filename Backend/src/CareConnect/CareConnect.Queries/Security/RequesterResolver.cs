using System.Data;
using CareConnect.DTOs.Errors;
using Dapper;

namespace CareConnect.Queries.Security;

/// <summary>
/// Resolves "who is actually calling" from the DB, so authorization decisions are based on real
/// relationships (User → Client / User → Caregiver → CaregiverAssignment), never on a URL Id
/// taken at face value.
/// </summary>
public static class RequesterResolver
{
    private const string Sql = """
        SELECT u.Id AS UserId, u.Role, cl.Id AS ClientId, cg.Id AS CaregiverId
        FROM Users u
        LEFT JOIN Clients cl ON cl.UserId = u.Id
        LEFT JOIN Caregivers cg ON cg.UserId = u.Id
        WHERE u.Auth0UserId = @Auth0UserId;
        """;

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
