-- One batch, one round trip. The handler reads these result sets BY POSITION via
-- QueryMultipleAsync — adding, removing, or reordering a SELECT here means updating the handler.
--
-- Hours/earnings math stays in decimal: DATEDIFF(SECOND, ...) is cast to DECIMAL before dividing
-- by 3600, never float — the same "no floating-point money math" rule as
-- CaregiverEarningsCalculator, done here in SQL because it aggregates across many visits.

-- 1. Today's visits
SELECT v.Id AS VisitId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
       clu.FirstName + ' ' + clu.LastName AS ClientFullName,
       v.ScheduledStartUtc, v.ScheduledEndUtc, v.Status
FROM Visits v
INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
INNER JOIN Users cgu ON cgu.Id = cg.UserId
INNER JOIN Clients cl ON cl.Id = a.ClientId
INNER JOIN Users clu ON clu.Id = cl.UserId
WHERE a.CaregiverId = @CaregiverId AND CAST(v.ScheduledStartUtc AS date) = @Today
ORDER BY v.ScheduledStartUtc;

-- 2. Upcoming scheduled visits
SELECT v.Id AS VisitId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
       clu.FirstName + ' ' + clu.LastName AS ClientFullName,
       v.ScheduledStartUtc, v.ScheduledEndUtc, v.Status
FROM Visits v
INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
INNER JOIN Users cgu ON cgu.Id = cg.UserId
INNER JOIN Clients cl ON cl.Id = a.ClientId
INNER JOIN Users clu ON clu.Id = cl.UserId
WHERE a.CaregiverId = @CaregiverId AND v.Status = @Scheduled AND v.ScheduledStartUtc > @NowUtc
ORDER BY v.ScheduledStartUtc;

-- 3. Completed visit count, hours, and actual earnings
SELECT
    COUNT(*) AS CompletedVisitCount,
    ISNULL(SUM(CAST(DATEDIFF(SECOND, v.ActualStartUtc, v.ActualEndUtc) AS DECIMAL(18,4)) / 3600.0), 0) AS TotalHoursWorked,
    ISNULL(SUM(CAST(DATEDIFF(SECOND, v.ActualStartUtc, v.ActualEndUtc) AS DECIMAL(18,4)) / 3600.0 * c.HourlyRate), 0) AS ActualEarnings
FROM Visits v
INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
INNER JOIN Caregivers c ON c.Id = a.CaregiverId
WHERE a.CaregiverId = @CaregiverId AND v.Status = @Completed
  AND v.ActualStartUtc IS NOT NULL AND v.ActualEndUtc IS NOT NULL;

-- 4. Estimated earnings from still-scheduled visits (scheduled window, not actual)
SELECT
    ISNULL(SUM(CAST(DATEDIFF(SECOND, v.ScheduledStartUtc, v.ScheduledEndUtc) AS DECIMAL(18,4)) / 3600.0 * c.HourlyRate), 0)
        AS EstimatedUpcomingEarnings
FROM Visits v
INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
INNER JOIN Caregivers c ON c.Id = a.CaregiverId
WHERE a.CaregiverId = @CaregiverId AND v.Status = @Scheduled;
