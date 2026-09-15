using CareConnect.DTOs.CareTasks;
using MediatR;

namespace CareConnect.Queries.CareTasks.GetCareTaskById;

public sealed record GetCareTaskByIdQuery(int CareTaskId) : IRequest<CareTaskDto?>;
