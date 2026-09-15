SELECT a.CaregiverId, a.ClientId
FROM Visits v
INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
WHERE v.Id = @VisitId;
