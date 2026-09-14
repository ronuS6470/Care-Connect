using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.Visits;

/// <summary>
/// Resource-level authorization for the check-in/check-out/complete workflow: resolves who is
/// actually calling from their Auth0 "sub" claim, then verifies they're the caregiver this
/// specific visit is assigned to. A caregiver cannot act on another caregiver's visit just by
/// putting a different visit Id in the URL — the comparison is against the visit's real assigned
/// CaregiverId, not anything the caller supplied.
/// </summary>
internal static class VisitOwnership
{
    public static async Task<Caregiver> EnsureCallerOwnsVisitAsync(
        CareConnectDbContext dbContext,
        Visit visit,
        string requestingAuth0UserId,
        CancellationToken cancellationToken)
    {
        var caregiver = await dbContext.Caregivers
            .FirstOrDefaultAsync(c => c.User.Auth0UserId == requestingAuth0UserId, cancellationToken)
            ?? throw new ForbiddenException("This account is not recognized as a caregiver.");

        if (caregiver.Id != visit.CaregiverAssignment.CaregiverId)
        {
            throw new ForbiddenException("You may only act on your own visits.");
        }

        return caregiver;
    }
}
