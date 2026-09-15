-- One batch, one round trip. The handler reads these result sets BY POSITION via
-- QueryMultipleAsync — adding, removing, or reordering a SELECT here means updating the handler.
--
-- Day-of-week is deliberately NOT computed here: DATEPART(WEEKDAY, ...) depends on the session's
-- SET DATEFIRST, which isn't guaranteed, and would silently misalign with
-- CaregiverAvailabilities.DayOfWeek (stored in .NET's Sunday=0..Saturday=6). The handler fetches
-- raw rows and compares DayOfWeek in C# (DateTime.DayOfWeek) instead.

-- 1. Active (scheduled / in-progress) visits
WITH ActiveVisits AS (
    SELECT v.Id AS VisitId, a.CaregiverId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
           v.ScheduledStartUtc, v.ScheduledEndUtc
    FROM Visits v
    INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
    INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
    INNER JOIN Users cgu ON cgu.Id = cg.UserId
    WHERE v.Status IN (@Scheduled, @InProgress)
)
SELECT VisitId, CaregiverId, CaregiverFullName, ScheduledStartUtc, ScheduledEndUtc
FROM ActiveVisits
ORDER BY ScheduledStartUtc;

-- 2. Pairs of active visits that overlap for the same caregiver (each pair once)
WITH ActiveVisits AS (
    SELECT v.Id AS VisitId, a.CaregiverId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
           v.ScheduledStartUtc, v.ScheduledEndUtc
    FROM Visits v
    INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
    INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
    INNER JOIN Users cgu ON cgu.Id = cg.UserId
    WHERE v.Status IN (@Scheduled, @InProgress)
)
SELECT av1.CaregiverId, av1.CaregiverFullName, av1.VisitId, av1.ScheduledStartUtc, av1.ScheduledEndUtc,
       av2.VisitId AS ConflictingVisitId
FROM ActiveVisits av1
INNER JOIN ActiveVisits av2
    ON av2.CaregiverId = av1.CaregiverId
   AND av2.VisitId <> av1.VisitId
   AND av1.ScheduledStartUtc < av2.ScheduledEndUtc
   AND av1.ScheduledEndUtc > av2.ScheduledStartUtc
   AND av1.VisitId < av2.VisitId;

-- 3. Active availability windows
SELECT CaregiverId, DayOfWeek, StartTime, EndTime
FROM CaregiverAvailabilities
WHERE IsActive = 1;
