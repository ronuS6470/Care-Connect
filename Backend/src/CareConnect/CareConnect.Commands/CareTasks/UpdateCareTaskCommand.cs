using CareConnect.DTOs.CareTasks;
using MediatR;

namespace CareConnect.Commands.CareTasks;

public sealed record UpdateCareTaskCommand(int CareTaskId, UpdateCareTaskDto CareTask) : IRequest;
