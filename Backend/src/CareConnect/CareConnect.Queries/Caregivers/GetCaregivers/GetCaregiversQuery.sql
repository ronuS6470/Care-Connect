SELECT c.Id, c.UserId, u.FirstName + ' ' + u.LastName AS FullName, u.Email, u.PhoneNumber,
       c.LicenseNumber, c.HourlyRate, c.HireDate, c.DateOfBirth, c.YearsOfExperience, c.IsActive
FROM Caregivers c
INNER JOIN Users u ON u.Id = c.UserId
ORDER BY c.Id
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
