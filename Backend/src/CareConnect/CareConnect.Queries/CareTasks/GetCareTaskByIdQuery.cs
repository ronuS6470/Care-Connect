using CareConnect.DTOs.CareTasks;
using MediatR;

namespace CareConnect.Queries.CareTasks;

public sealed record GetCareTaskByIdQuery(int CareTaskId) : IRequest<CareTaskDto?>;
