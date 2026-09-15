WITH CaregiverCompletedCounts AS (
    SELECT cg.Id AS CaregiverId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
           COUNT(*) AS CompletedVisitCount
    FROM Visits v
    INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
    INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
    INNER JOIN Users cgu ON cgu.Id = cg.UserId
    WHERE v.Status = @Completed AND v.ActualStartUtc IS NOT NULL AND v.ActualEndUtc IS NOT NULL
      AND CAST(v.ActualStartUtc AS date) BETWEEN @FromDate AND @ToDate
    GROUP BY cg.Id, cgu.FirstName, cgu.LastName
)
SELECT TOP (@TopN)
    CAST(ROW_NUMBER() OVER (ORDER BY CompletedVisitCount DESC) AS INT) AS Rank,
    CaregiverId, CaregiverFullName, CompletedVisitCount
FROM CaregiverCompletedCounts
ORDER BY CompletedVisitCount DESC;
