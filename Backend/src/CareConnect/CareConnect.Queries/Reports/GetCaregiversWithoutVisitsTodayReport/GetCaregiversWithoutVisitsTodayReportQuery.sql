SELECT c.Id AS CaregiverId, u.FirstName + ' ' + u.LastName AS CaregiverFullName
FROM Caregivers c
INNER JOIN Users u ON u.Id = c.UserId
WHERE c.IsActive = 1
  AND NOT EXISTS (
      SELECT 1
      FROM Visits v
      INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
      WHERE a.CaregiverId = c.Id AND CAST(v.ScheduledStartUtc AS date) = @Today
  )
ORDER BY CaregiverFullName;
