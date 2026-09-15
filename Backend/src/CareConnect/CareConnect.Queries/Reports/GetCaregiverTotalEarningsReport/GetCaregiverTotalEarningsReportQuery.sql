-- Hours use actual check-in/check-out, in decimal: DATEDIFF(SECOND, ...) is cast to DECIMAL before
-- dividing by 3600, never float — so earnings never touch floating-point arithmetic.
SELECT cg.Id AS CaregiverId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
       cg.HourlyRate,
       SUM(CAST(DATEDIFF(SECOND, v.ActualStartUtc, v.ActualEndUtc) AS DECIMAL(18,4)) / 3600.0) AS TotalHoursWorked,
       SUM(CAST(DATEDIFF(SECOND, v.ActualStartUtc, v.ActualEndUtc) AS DECIMAL(18,4)) / 3600.0 * cg.HourlyRate) AS TotalEarnings
FROM Visits v
INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
INNER JOIN Users cgu ON cgu.Id = cg.UserId
WHERE v.Status = @Completed AND v.ActualStartUtc IS NOT NULL AND v.ActualEndUtc IS NOT NULL
  AND CAST(v.ActualStartUtc AS date) BETWEEN @FromDate AND @ToDate
GROUP BY cg.Id, cgu.FirstName, cgu.LastName, cg.HourlyRate
ORDER BY TotalEarnings DESC;
