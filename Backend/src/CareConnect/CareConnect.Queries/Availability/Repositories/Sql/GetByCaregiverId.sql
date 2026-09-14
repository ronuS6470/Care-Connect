SELECT Id, CaregiverId, DayOfWeek, StartTime, EndTime, IsActive
FROM CaregiverAvailabilities
WHERE CaregiverId = @CaregiverId
ORDER BY DayOfWeek, StartTime;
