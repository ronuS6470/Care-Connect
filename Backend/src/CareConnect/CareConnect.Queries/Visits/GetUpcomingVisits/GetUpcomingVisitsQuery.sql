-- Row scoping by @RequesterRole (CareConnect.DTOs.Enums.UserRole): 1 Admin sees every visit,
-- 2 Caregiver sees only visits on their own assignments, 3 Client sees only visits on theirs.
-- Any other role matches no branch and gets no rows.
-- OPTION (RECOMPILE): one statement serves all three roles, so a cached plan compiled for one
-- role would be reused by the others; recompiling lets each call prune the branches it can't hit.
-- ORDER BY includes v.Id as a tie-breaker: many visits share a start time, and OFFSET/FETCH paging over a
-- non-unique sort key can repeat a row on two pages or skip it entirely.
SELECT v.Id AS VisitId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
       clu.FirstName + ' ' + clu.LastName AS ClientFullName,
       v.ScheduledStartUtc, v.ScheduledEndUtc, v.Status
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
ORDER BY v.ScheduledStartUtc, v.Id
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
OPTION (RECOMPILE);
