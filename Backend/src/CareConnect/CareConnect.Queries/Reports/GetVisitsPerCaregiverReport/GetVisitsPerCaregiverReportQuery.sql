SELECT cg.Id AS CaregiverId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
       COUNT(*) AS TotalVisits,
       SUM(CASE WHEN v.Status = @Completed THEN 1 ELSE 0 END) AS CompletedVisits,
       SUM(CASE WHEN v.Status = @Cancelled THEN 1 ELSE 0 END) AS CancelledVisits,
       SUM(CASE WHEN v.Status = @NoShow THEN 1 ELSE 0 END) AS NoShowVisits
FROM Visits v
INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
INNER JOIN Users cgu ON cgu.Id = cg.UserId
WHERE CAST(v.ScheduledStartUtc AS date) BETWEEN @FromDate AND @ToDate
GROUP BY cg.Id, cgu.FirstName, cgu.LastName
ORDER BY TotalVisits DESC;
