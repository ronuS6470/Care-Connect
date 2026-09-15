SELECT c.Id, c.UserId, u.FirstName + ' ' + u.LastName AS FullName, u.Email, u.PhoneNumber,
       c.LicenseNumber, c.HourlyRate, c.HireDate, c.DateOfBirth, c.YearsOfExperience, c.IsActive
FROM Caregivers c
INNER JOIN Users u ON u.Id = c.UserId
WHERE c.Id = @CaregiverId;
