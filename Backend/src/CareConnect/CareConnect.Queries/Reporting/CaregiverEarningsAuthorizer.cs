using CareConnect.DTOs.Enums;
using CareConnect.Queries.Security;

namespace CareConnect.Queries.Reporting;

/// <summary>
/// Pure authorization decision for viewing a caregiver's earnings/hours — no I/O, independently
/// testable. Admin sees any caregiver; a Caregiver sees only themselves; a Client sees none.
/// </summary>
public static class CaregiverEarningsAuthorizer
{
    public static bool CanView(RequesterContext requester, int targetCaregiverId) =>
        requester.Role switch
        {
            UserRole.Admin => true,
            UserRole.Caregiver => requester.CaregiverId == targetCaregiverId,
            _ => false,
        };
}
