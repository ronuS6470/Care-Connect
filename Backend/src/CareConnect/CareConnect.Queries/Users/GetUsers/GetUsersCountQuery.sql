-- Same filters as GetUsersQuery.sql — keep the two WHERE clauses identical.
SELECT COUNT(*)
FROM Users u
WHERE (@Search IS NULL OR u.FirstName LIKE '%' + @Search + '%' OR u.LastName LIKE '%' + @Search + '%' OR u.Email LIKE '%' + @Search + '%')
  AND (@Role IS NULL OR u.Role = @Role)
  AND (@IsActive IS NULL OR u.IsActive = @IsActive);
