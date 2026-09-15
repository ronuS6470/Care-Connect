-- Shared: used by both GetCaregiverEarnings and GetCaregiverHours.
--
-- Filtered by ActualStartUtc, not ScheduledStartUtc — "date range" means the period actually
-- worked, matching the rule that earnings/hours are computed from actual check-in/check-out.
-- A visit with no ActualStartUtc yet (Scheduled) can never match; one that was checked in and
-- then cancelled before checkout is still fetched (it has an ActualStartUtc) but contributes
-- nothing — CaregiverEarningsCalculator.FilterCompleted is what actually decides that.
SELECT v.Id AS VisitId, clu.FirstName + ' ' + clu.LastName AS ClientFullName,
       v.Status, v.ActualStartUtc, v.ActualEndUtc
FROM Visits v
INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
INNER JOIN Clients cl ON cl.Id = a.ClientId
INNER JOIN Users clu ON clu.Id = cl.UserId
WHERE a.CaregiverId = @CaregiverId
  AND v.ActualStartUtc IS NOT NULL
  AND CAST(v.ActualStartUtc AS date) BETWEEN @FromDate AND @ToDate
ORDER BY v.ActualStartUtc;
