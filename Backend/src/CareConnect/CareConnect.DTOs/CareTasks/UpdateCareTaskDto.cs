namespace CareConnect.DTOs.CareTasks;

public sealed class UpdateCareTaskDto
{
    public required string Name { get; init; }

    public string? Description { get; init; }

    public required bool IsActive { get; init; }
}
