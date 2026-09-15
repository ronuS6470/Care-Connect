-- Shared: used by both GetCaregiverEarnings and GetCaregiverHours.
SELECT c.Id, c.HourlyRate, u.FirstName + ' ' + u.LastName AS FullName
FROM Caregivers c
INNER JOIN Users u ON u.Id = c.UserId
WHERE c.Id = @CaregiverId;
