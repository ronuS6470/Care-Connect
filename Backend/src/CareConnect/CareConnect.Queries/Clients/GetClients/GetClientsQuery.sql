-- Row scoping by @RequesterRole (CareConnect.DTOs.Enums.UserRole): 1 Admin sees every client,
-- 3 Client sees only their own row, 2 Caregiver sees only clients they hold an assignment with.
-- Any other role matches no branch and gets no rows.
-- OPTION (RECOMPILE): one statement serves all three roles, so a cached plan compiled for one
-- role would be reused by the others; recompiling lets each call prune the branches it can't hit.
SELECT c.Id, c.UserId, u.FirstName + ' ' + u.LastName AS FullName, u.Email, u.PhoneNumber,
       c.AddressLine1, c.AddressLine2, c.City, c.State, c.PostalCode,
       c.EmergencyContactName, c.EmergencyContactPhone, c.IsActive
FROM Clients c
INNER JOIN Users u ON u.Id = c.UserId
WHERE (
         @RequesterRole = 1
      OR (@RequesterRole = 3 AND c.Id = @RequesterClientId)
      OR (@RequesterRole = 2 AND c.Id IN (SELECT ClientId FROM CaregiverAssignments WHERE CaregiverId = @RequesterCaregiverId))
      )
  AND (@Search IS NULL OR u.FirstName LIKE '%' + @Search + '%' OR u.LastName LIKE '%' + @Search + '%' OR u.Email LIKE '%' + @Search + '%')
  AND (@IsActive IS NULL OR c.IsActive = @IsActive)
ORDER BY c.Id
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
OPTION (RECOMPILE);
