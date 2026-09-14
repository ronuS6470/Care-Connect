namespace CareConnect.DTOs.CareTasks;

public sealed class CreateCareTaskDto
{
    public required string Name { get; init; }

    public string? Description { get; init; }
}
