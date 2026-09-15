SELECT ct.Id AS CareTaskId, ct.Name AS CareTaskName,
       COUNT(*) AS TimesRequested,
       SUM(CASE WHEN vt.IsCompleted = 1 THEN 1 ELSE 0 END) AS TimesCompleted
FROM VisitTasks vt
INNER JOIN CareTasks ct ON ct.Id = vt.CareTaskId
INNER JOIN Visits v ON v.Id = vt.VisitId
WHERE CAST(v.ScheduledStartUtc AS date) BETWEEN @FromDate AND @ToDate
GROUP BY ct.Id, ct.Name
ORDER BY TimesRequested DESC;
