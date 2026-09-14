SELECT COUNT(*) FROM CareTasks WHERE (@IsActive IS NULL OR IsActive = @IsActive);
