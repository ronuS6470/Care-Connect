using CareConnect.DTOs.CareTasks;
using CareConnect.DTOs.Common;
using MediatR;

namespace CareConnect.Queries.CareTasks.GetCareTasks;

public sealed record GetCareTasksQuery(int Page, int PageSize, bool? IsActive) : IRequest<PagedResponseDto<CareTaskDto>>;
