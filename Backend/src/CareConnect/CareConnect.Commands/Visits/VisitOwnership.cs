using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Errors;

namespace CareConnect.Commands.Visits;

/// <summary>
/// Resource-level authorization for the check-in/check-out/complete workflow: verifies the caller
/// (already resolved as a Caregiver by the repository) is the caregiver this specific visit is
/// assigned to. A caregiver cannot act on another caregiver's visit just by putting a different
/// visit Id in the URL — the comparison is against the visit's real assigned CaregiverId, not
/// anything the caller supplied.
/// </summary>
internal static class VisitOwnership
{
    public static Caregiver EnsureCallerOwnsVisit(Caregiver caregiver, Visit visit)
    {
        if (caregiver.Id != visit.CaregiverAssignment.CaregiverId)
        {
            throw new ForbiddenException("You may only act on your own visits.");
        }

        return caregiver;
    }
}
