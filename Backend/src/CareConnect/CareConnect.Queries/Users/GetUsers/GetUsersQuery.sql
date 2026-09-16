-- HasPassword reports whether a password is set, never the hash itself — no password material
-- leaves the database through this query.
-- LEFT JOINs expose whether the account has the profile its role implies; a user can hold
-- Role=Caregiver with no Caregivers row, since creating that profile requires the role first.
SELECT u.Id,
       u.Email,
       u.FirstName + ' ' + u.LastName AS FullName,
       u.PhoneNumber,
       u.Role,
       u.IsActive,
       CAST(CASE WHEN u.PasswordHash IS NULL THEN 0 ELSE 1 END AS BIT) AS HasPassword,
       cg.Id AS CaregiverId,
       cl.Id AS ClientId,
       u.CreatedAtUtc
FROM Users u
LEFT JOIN Caregivers cg ON cg.UserId = u.Id
LEFT JOIN Clients cl ON cl.UserId = u.Id
WHERE (@Search IS NULL OR u.FirstName LIKE '%' + @Search + '%' OR u.LastName LIKE '%' + @Search + '%' OR u.Email LIKE '%' + @Search + '%')
  AND (@Role IS NULL OR u.Role = @Role)
  AND (@IsActive IS NULL OR u.IsActive = @IsActive)
ORDER BY u.Id
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
