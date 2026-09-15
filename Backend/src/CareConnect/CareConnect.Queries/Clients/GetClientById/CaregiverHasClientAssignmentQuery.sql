SELECT CASE WHEN EXISTS (
    SELECT 1 FROM CaregiverAssignments WHERE CaregiverId = @CaregiverId AND ClientId = @ClientId
) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END;
