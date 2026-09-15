SELECT c.Id AS ClientId, u.FirstName + ' ' + u.LastName AS ClientFullName, c.IsActive
FROM Clients c
INNER JOIN Users u ON u.Id = c.UserId
LEFT JOIN CaregiverAssignments a ON a.ClientId = c.Id AND a.Status = @ActiveAssignment
WHERE a.Id IS NULL
ORDER BY c.IsActive DESC, ClientFullName;
