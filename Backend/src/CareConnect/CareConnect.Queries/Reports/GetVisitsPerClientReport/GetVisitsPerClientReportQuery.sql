SELECT cl.Id AS ClientId, clu.FirstName + ' ' + clu.LastName AS ClientFullName,
       COUNT(*) AS TotalVisits,
       SUM(CASE WHEN v.Status = @Completed THEN 1 ELSE 0 END) AS CompletedVisits,
       SUM(CASE WHEN v.Status = @Cancelled THEN 1 ELSE 0 END) AS CancelledVisits,
       SUM(CASE WHEN v.Status = @NoShow THEN 1 ELSE 0 END) AS NoShowVisits
FROM Visits v
INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
INNER JOIN Clients cl ON cl.Id = a.ClientId
INNER JOIN Users clu ON clu.Id = cl.UserId
WHERE CAST(v.ScheduledStartUtc AS date) BETWEEN @FromDate AND @ToDate
GROUP BY cl.Id, clu.FirstName, clu.LastName
ORDER BY TotalVisits DESC;
