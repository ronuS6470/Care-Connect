SELECT v.Id AS VisitId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
       clu.FirstName + ' ' + clu.LastName AS ClientFullName,
       v.ScheduledStartUtc, v.ScheduledEndUtc, v.Status
FROM Visits v
INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
INNER JOIN Users cgu ON cgu.Id = cg.UserId
INNER JOIN Clients cl ON cl.Id = a.ClientId
INNER JOIN Users clu ON clu.Id = cl.UserId
WHERE v.Status = @Completed
  AND CAST(v.ScheduledStartUtc AS date) BETWEEN @FromDate AND @ToDate
ORDER BY v.ScheduledStartUtc;
