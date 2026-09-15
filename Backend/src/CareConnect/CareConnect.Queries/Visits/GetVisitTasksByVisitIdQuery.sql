-- Shared: used by both GetVisitById (to embed a visit's tasks) and GetVisitTasks.
SELECT vt.Id, vt.VisitId, vt.CareTaskId, ct.Name AS CareTaskName,
       vt.IsCompleted, vt.CompletedAtUtc, vt.Notes
FROM VisitTasks vt
INNER JOIN CareTasks ct ON ct.Id = vt.CareTaskId
WHERE vt.VisitId = @VisitId
ORDER BY vt.Id;
