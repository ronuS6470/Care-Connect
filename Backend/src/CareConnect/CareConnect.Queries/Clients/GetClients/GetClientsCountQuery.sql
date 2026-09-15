-- Same row scoping and filters as GetClientsQuery.sql — keep the two WHERE clauses identical.
SELECT COUNT(*)
FROM Clients c
INNER JOIN Users u ON u.Id = c.UserId
WHERE (
         @RequesterRole = 1
      OR (@RequesterRole = 3 AND c.Id = @RequesterClientId)
      OR (@RequesterRole = 2 AND c.Id IN (SELECT ClientId FROM CaregiverAssignments WHERE CaregiverId = @RequesterCaregiverId))
      )
  AND (@Search IS NULL OR u.FirstName LIKE '%' + @Search + '%' OR u.LastName LIKE '%' + @Search + '%' OR u.Email LIKE '%' + @Search + '%')
  AND (@IsActive IS NULL OR c.IsActive = @IsActive)
OPTION (RECOMPILE);
