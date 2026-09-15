-- Row scoping by @RequesterRole (CareConnect.DTOs.Enums.UserRole): 1 Admin sees every assignment,
-- 2 Caregiver sees only their own, 3 Client sees only their own. Any other role gets no rows.
-- OPTION (RECOMPILE): one statement serves all three roles, so a cached plan compiled for one
-- role would be reused by the others; recompiling lets each call prune the branches it can't hit.
SELECT a.Id, a.CaregiverId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
       a.ClientId, clu.FirstName + ' ' + clu.LastName AS ClientFullName,
       a.Status, a.StartDate, a.EndDate, a.Notes
FROM CaregiverAssignments a
INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
INNER JOIN Users cgu ON cgu.Id = cg.UserId
INNER JOIN Clients cl ON cl.Id = a.ClientId
INNER JOIN Users clu ON clu.Id = cl.UserId
WHERE (
         @RequesterRole = 1
      OR (@RequesterRole = 2 AND a.CaregiverId = @RequesterCaregiverId)
      OR (@RequesterRole = 3 AND a.ClientId = @RequesterClientId)
      )
  AND (@Status IS NULL OR a.Status = @Status)
ORDER BY a.Id
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
OPTION (RECOMPILE);
