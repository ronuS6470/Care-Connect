-- One batch, one round trip. The handler reads these result sets BY POSITION via
-- QueryMultipleAsync — adding, removing, or reordering a SELECT here means updating the handler.
--
-- Hours/earnings math stays in decimal: DATEDIFF(SECOND, ...) is cast to DECIMAL before dividing
-- by 3600, never float — the same "no floating-point money math" rule as
-- CaregiverEarningsCalculator, done here in SQL because it aggregates across many visits.

-- 1. Total clients
SELECT COUNT(*) FROM Clients;

-- 2. Active caregivers
SELECT COUNT(*) FROM Caregivers WHERE IsActive = 1;

-- 3. Today's visit breakdown
SELECT
    COUNT(*) AS TodaysVisitCount,
    SUM(CASE WHEN Status = @Completed THEN 1 ELSE 0 END) AS CompletedVisitsToday,
    SUM(CASE WHEN Status IN (@Scheduled, @InProgress) THEN 1 ELSE 0 END) AS PendingVisitsToday,
    SUM(CASE WHEN Status IN (@Cancelled, @NoShow) THEN 1 ELSE 0 END) AS CancelledVisitsToday
FROM Visits
WHERE CAST(ScheduledStartUtc AS date) = @Today;

-- 4. All-time completed hours and earnings
SELECT
    ISNULL(SUM(CAST(DATEDIFF(SECOND, v.ActualStartUtc, v.ActualEndUtc) AS DECIMAL(18,4)) / 3600.0), 0) AS TotalHoursWorked,
    ISNULL(SUM(CAST(DATEDIFF(SECOND, v.ActualStartUtc, v.ActualEndUtc) AS DECIMAL(18,4)) / 3600.0 * c.HourlyRate), 0) AS TotalCaregiverEarnings
FROM Visits v
INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
INNER JOIN Caregivers c ON c.Id = a.CaregiverId
WHERE v.Status = @Completed AND v.ActualStartUtc IS NOT NULL AND v.ActualEndUtc IS NOT NULL;
