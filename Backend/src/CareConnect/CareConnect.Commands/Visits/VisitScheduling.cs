using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Errors;
using CareConnect.Infrastructure.Entities;
using CareConnect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareConnect.Commands.Visits;

/// <summary>
/// Business rules shared by Create and Update visit — kept in one place so scheduling a visit and
/// rescheduling one can never drift apart on what "valid" means.
/// </summary>
internal static class VisitScheduling
{
    /// <summary>Rules 3 &amp; 5: the assignment must be Active and cover the visit's date.</summary>
    public static void EnsureAssignmentActiveForDate(CaregiverAssignment assignment, DateOnly visitDate)
    {
        if (assignment.Status != AssignmentStatus.Active)
        {
            throw new BusinessRuleViolationException("The assignment must be active to schedule a visit against it.");
        }

        if (visitDate < assignment.StartDate || (assignment.EndDate.HasValue && visitDate > assignment.EndDate.Value))
        {
            throw new BusinessRuleViolationException("The assignment is not active for the visit's date.");
        }
    }

    /// <summary>Rule 1: the visit must fall entirely inside one of the caregiver's availability windows.</summary>
    public static async Task EnsureWithinAvailabilityAsync(
        CareConnectDbContext dbContext,
        int caregiverId,
        DateTime scheduledStartUtc,
        DateTime scheduledEndUtc,
        CancellationToken cancellationToken)
    {
        var dayOfWeek = scheduledStartUtc.DayOfWeek;
        var startTime = TimeOnly.FromDateTime(scheduledStartUtc);
        var endTime = TimeOnly.FromDateTime(scheduledEndUtc);

        var fitsWithinAWindow = await dbContext.CaregiverAvailabilities.AnyAsync(a =>
            a.CaregiverId == caregiverId &&
            a.DayOfWeek == dayOfWeek &&
            a.IsActive &&
            a.StartTime <= startTime &&
            a.EndTime >= endTime,
            cancellationToken);

        if (!fitsWithinAWindow)
        {
            throw new BusinessRuleViolationException(
                "The visit falls outside the caregiver's configured availability for that day.");
        }
    }

    /// <summary>Rule 2: no overlapping visit for the same caregiver, across any of their assignments.</summary>
    public static async Task EnsureNoOverlapAsync(
        CareConnectDbContext dbContext,
        int caregiverId,
        DateTime scheduledStartUtc,
        DateTime scheduledEndUtc,
        int? excludingVisitId,
        CancellationToken cancellationToken)
    {
        var hasOverlap = await dbContext.Visits.AnyAsync(v =>
            v.Id != (excludingVisitId ?? -1) &&
            v.CaregiverAssignment.CaregiverId == caregiverId &&
            v.Status != VisitStatus.Cancelled &&
            v.ScheduledStartUtc < scheduledEndUtc &&
            v.ScheduledEndUtc > scheduledStartUtc,
            cancellationToken);

        if (hasOverlap)
        {
            throw new BusinessRuleViolationException("This caregiver already has an overlapping visit.");
        }
    }
}
