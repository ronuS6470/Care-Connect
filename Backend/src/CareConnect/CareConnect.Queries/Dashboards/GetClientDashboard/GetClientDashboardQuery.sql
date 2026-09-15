-- One batch, one round trip. The handler reads these result sets BY POSITION via
-- QueryMultipleAsync — adding, removing, or reordering a SELECT here means updating the handler.

-- 1. Caregivers with an active assignment to this client
SELECT DISTINCT c.Id AS CaregiverId, cgu.FirstName + ' ' + cgu.LastName AS FullName, cgu.PhoneNumber
FROM CaregiverAssignments a
INNER JOIN Caregivers c ON c.Id = a.CaregiverId
INNER JOIN Users cgu ON cgu.Id = c.UserId
WHERE a.ClientId = @ClientId AND a.Status = @ActiveAssignment;

-- 2. Next scheduled visit (zero or one row)
SELECT TOP (1) v.Id AS VisitId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
       clu.FirstName + ' ' + clu.LastName AS ClientFullName,
       v.ScheduledStartUtc, v.ScheduledEndUtc, v.Status
FROM Visits v
INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
INNER JOIN Users cgu ON cgu.Id = cg.UserId
INNER JOIN Clients cl ON cl.Id = a.ClientId
INNER JOIN Users clu ON clu.Id = cl.UserId
WHERE a.ClientId = @ClientId AND v.Status = @Scheduled AND v.ScheduledStartUtc > @NowUtc
ORDER BY v.ScheduledStartUtc;

-- 3. All upcoming scheduled visits
SELECT v.Id AS VisitId, cgu.FirstName + ' ' + cgu.LastName AS CaregiverFullName,
       clu.FirstName + ' ' + clu.LastName AS ClientFullName,
       v.ScheduledStartUtc, v.ScheduledEndUtc, v.Status
FROM Visits v
INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
INNER JOIN Caregivers cg ON cg.Id = a.CaregiverId
INNER JOIN Users cgu ON cgu.Id = cg.UserId
INNER JOIN Clients cl ON cl.Id = a.ClientId
INNER JOIN Users clu ON clu.Id = cl.UserId
WHERE a.ClientId = @ClientId AND v.Status = @Scheduled AND v.ScheduledStartUtc > @NowUtc
ORDER BY v.ScheduledStartUtc;

-- 4. Completed visit count
SELECT COUNT(*)
FROM Visits v
INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
WHERE a.ClientId = @ClientId AND v.Status = @Completed;

-- 5. Ten most recent visit notes
SELECT TOP (10) n.Id, n.VisitId, n.AuthorUserId, u.FirstName + ' ' + u.LastName AS AuthorFullName,
       n.Content, n.CreatedAtUtc
FROM VisitNotes n
INNER JOIN Visits v ON v.Id = n.VisitId
INNER JOIN CaregiverAssignments a ON a.Id = v.CaregiverAssignmentId
INNER JOIN Users u ON u.Id = n.AuthorUserId
WHERE a.ClientId = @ClientId
ORDER BY n.CreatedAtUtc DESC;
