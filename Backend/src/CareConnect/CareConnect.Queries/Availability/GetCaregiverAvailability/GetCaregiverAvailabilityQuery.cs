using CareConnect.DTOs.Caregivers;
using MediatR;

namespace CareConnect.Queries.Availability.GetCaregiverAvailability;

public sealed record GetCaregiverAvailabilityQuery(int CaregiverId) : IRequest<IReadOnlyList<CaregiverAvailabilityDto>>;
