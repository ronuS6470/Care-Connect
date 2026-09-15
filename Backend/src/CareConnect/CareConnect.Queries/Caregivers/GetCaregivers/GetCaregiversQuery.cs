using CareConnect.DTOs.Caregivers;
using CareConnect.DTOs.Common;
using MediatR;

namespace CareConnect.Queries.Caregivers.GetCaregivers;

public sealed record GetCaregiversQuery(int Page, int PageSize) : IRequest<PagedResponseDto<CaregiverDto>>;
