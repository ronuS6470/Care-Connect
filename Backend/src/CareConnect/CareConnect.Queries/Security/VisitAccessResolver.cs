using System.Data;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Errors;
using Dapper;

namespace CareConnect.Queries.Security;

/// <summary>
/// Read-access rule for a visit's sub-resources (tasks, notes): Admin sees everything, the
/// assigned caregiver and the owning client can each see their own — resolved from the visit's
/// actual assignment, never from anything the caller supplied.
/// </summary>
public static class VisitAccessResolver
{
    private const string VisitOwnerSql = """
        SELECT a.CaregiverId, a.ClientId
        FROM Visits v
        INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
        WHERE v.Id = @VisitId;
        """;

    public static async Task<VisitOwnerRow> EnsureCanViewVisitAsync(
        IDbConnection connection,
        int visitId,
        RequesterContext requester,
        CancellationToken cancellationToken)
    {
        var owner = await connection.QuerySingleOrDefaultAsync<VisitOwnerRow>(new CommandDefinition(
            VisitOwnerSql,
            new { VisitId = visitId },
            cancellationToken: cancellationToken))
            ?? throw new NotFoundException($"Visit {visitId} was not found.");

        var canView = requester.Role switch
        {
            UserRole.Admin => true,
            UserRole.Caregiver => requester.CaregiverId == owner.CaregiverId,
            UserRole.Client => requester.ClientId == owner.ClientId,
            _ => false,
        };

        if (!canView)
        {
            throw new ForbiddenException("You do not have access to this visit.");
        }

        return owner;
    }
}

public sealed class VisitOwnerRow
{
    public int CaregiverId { get; init; }

    public int ClientId { get; init; }
}
