-- Same row scoping and filters as GetUpcomingVisitsQuery.sql — keep the two WHERE clauses identical.
SELECT COUNT(*)
FROM Visits v
INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
INNER JOIN Users cgu ON cgu.Id = cg.UserId
INNER JOIN Clients cl ON cl.Id = a.ClientId
INNER JOIN Users clu ON clu.Id = cl.UserId
WHERE (
         @RequesterRole = 1
      OR (@RequesterRole = 2 AND a.CaregiverId = @RequesterCaregiverId)
      OR (@RequesterRole = 3 AND a.ClientId = @RequesterClientId)
      )
  AND v.ScheduledStartUtc >= @NowUtc
  AND v.Status = @ScheduledStatus
OPTION (RECOMPILE);
