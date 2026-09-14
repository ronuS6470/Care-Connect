SELECT Id, Name, Description, IsActive
FROM CareTasks
WHERE (@IsActive IS NULL OR IsActive = @IsActive)
ORDER BY Name
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
