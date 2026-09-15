using System.Data;
using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Errors;
using CareConnect.Queries.Security;

namespace CareConnect.Queries.Reports;

/// <summary>Shared by every report call: all reports in this folder are Admin-only.</summary>
internal static class ReportAccess
{
    public static async Task EnsureAdminAsync(IDbConnection connection, string requestingAuth0UserId, CancellationToken cancellationToken)
    {
        var requester = await RequesterResolver.ResolveAsync(connection, requestingAuth0UserId, cancellationToken);

        if (requester.Role != UserRole.Admin)
        {
            throw new ForbiddenException("Only Admin may view this report.");
        }
    }
}
