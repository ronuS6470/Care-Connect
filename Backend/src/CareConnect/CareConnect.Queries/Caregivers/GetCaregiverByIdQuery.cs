using CareConnect.DTOs.Caregivers;
using MediatR;

namespace CareConnect.Queries.Caregivers;

public sealed record GetCaregiverByIdQuery(int CaregiverId) : IRequest<CaregiverDto?>;
