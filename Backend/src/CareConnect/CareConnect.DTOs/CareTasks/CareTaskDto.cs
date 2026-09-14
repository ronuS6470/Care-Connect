namespace CareConnect.DTOs.CareTasks;

public sealed class CareTaskDto
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    public string? Description { get; init; }

    public required bool IsActive { get; init; }
}
