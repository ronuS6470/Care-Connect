SELECT a.Id, a.CaregiverId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
       a.ClientId, clu.FirstName + ' ' + clu.LastName AS ClientFullName,
       a.Status, a.StartDate, a.EndDate, a.Notes
FROM CaregiverAssignments a
INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
INNER JOIN Users cgu ON cgu.Id = cg.UserId
INNER JOIN Clients cl ON cl.Id = a.ClientId
INNER JOIN Users clu ON clu.Id = cl.UserId
WHERE a.Id = @AssignmentId;
