using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Errors;

namespace CareConnect.Commands.Visits;

internal static class VisitTaskLookup
{
    public static void EnsureVisitIsEditable(Visit visit)
    {
        if (visit.Status is VisitStatus.Completed or VisitStatus.Cancelled or VisitStatus.NoShow)
        {
            throw new BusinessRuleException($"Cannot modify tasks on a visit with status {visit.Status}.");
        }
    }
}
