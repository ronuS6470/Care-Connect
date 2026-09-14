using CareConnect.DTOs.CareTasks;
using MediatR;

namespace CareConnect.Commands.CareTasks;

/// <summary>Creates a care task template. Returns the new task's Id.</summary>
public sealed record CreateCareTaskCommand(CreateCareTaskDto CareTask) : IRequest<int>;
