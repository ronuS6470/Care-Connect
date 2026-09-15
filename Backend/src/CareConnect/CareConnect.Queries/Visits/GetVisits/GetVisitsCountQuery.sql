-- Same row scoping and filters as GetVisitsQuery.sql — keep the two WHERE clauses identical.
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
  AND (@FromDate IS NULL OR CAST(v.ScheduledStartUtc AS date) >= @FromDate)
  AND (@ToDate IS NULL OR CAST(v.ScheduledStartUtc AS date) <= @ToDate)
  AND (@CaregiverId IS NULL OR a.CaregiverId = @CaregiverId)
  AND (@ClientId IS NULL OR a.ClientId = @ClientId)
  AND (@Status IS NULL OR v.Status = @Status)
  AND (@Search IS NULL OR cgu.FirstName LIKE '%' + @Search + '%' OR cgu.LastName LIKE '%' + @Search + '%'
       OR clu.FirstName LIKE '%' + @Search + '%' OR clu.LastName LIKE '%' + @Search + '%')
OPTION (RECOMPILE);
