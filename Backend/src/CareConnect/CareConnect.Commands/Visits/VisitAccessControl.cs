using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Errors;

namespace CareConnect.Commands.Visits;

/// <summary>
/// Write-access rule shared by VisitTask management and note-adding: only an Admin or the
/// specific caregiver a visit is assigned to may change it. Clients are read-only for both — they
/// receive care, they don't record it.
/// </summary>
internal static class VisitAccessControl
{
    public static void EnsureCanManageVisit(User user, Caregiver? caregiver, Visit visit)
    {
        if (user.Role == UserRole.Admin)
        {
            return;
        }

        if (user.Role == UserRole.Caregiver && caregiver is not null && caregiver.Id == visit.CaregiverAssignment.CaregiverId)
        {
            return;
        }

        throw new ForbiddenException("You do not have access to manage this visit.");
    }
}
