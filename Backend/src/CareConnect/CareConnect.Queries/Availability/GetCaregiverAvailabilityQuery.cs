using CareConnect.DTOs.Caregivers;
using MediatR;

namespace CareConnect.Queries.Availability;

public sealed record GetCaregiverAvailabilityQuery(int CaregiverId) : IRequest<IReadOnlyList<CaregiverAvailabilityDto>>;
