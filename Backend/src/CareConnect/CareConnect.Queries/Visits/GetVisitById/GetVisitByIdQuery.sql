SELECT v.Id, v.CaregiverAssignmentId, a.CaregiverId, a.ClientId,
       cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
       clu.FirstName + ' ' + clu.LastName AS ClientFullName,
       v.ScheduledStartUtc, v.ScheduledEndUtc, v.ActualStartUtc, v.ActualEndUtc,
       v.Status, v.CancellationReason
FROM Visits v
INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
INNER JOIN Users cgu ON cgu.Id = cg.UserId
INNER JOIN Clients cl ON cl.Id = a.ClientId
INNER JOIN Users clu ON clu.Id = cl.UserId
WHERE v.Id = @VisitId;
