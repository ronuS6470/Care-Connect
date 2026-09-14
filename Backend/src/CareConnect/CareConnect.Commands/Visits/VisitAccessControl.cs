using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.Visits;

/// <summary>
/// Write-access rule shared by VisitTask management and note-adding: only an Admin or the
/// specific caregiver a visit is assigned to may change it. Clients are read-only for both — they
/// receive care, they don't record it.
/// </summary>
internal static class VisitAccessControl
{
    public static async Task EnsureCanManageVisitAsync(
        CareConnectDbContext dbContext,
        Visit visit,
        string requestingAuth0UserId,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Auth0UserId == requestingAuth0UserId, cancellationToken)
            ?? throw new ForbiddenException("This account is not recognized.");

        if (user.Role == UserRole.Admin)
        {
            return;
        }

        if (user.Role == UserRole.Caregiver)
        {
            var caregiver = await dbContext.Caregivers
                .FirstOrDefaultAsync(c => c.UserId == user.Id, cancellationToken);

            if (caregiver is not null && caregiver.Id == visit.CaregiverAssignment.CaregiverId)
            {
                return;
            }
        }

        throw new ForbiddenException("You do not have access to manage this visit.");
    }
}
