using MediatR;

namespace CareConnect.Commands.Caregivers;

/// <summary>Soft-deletes (deactivates) a caregiver — historical visit/earning records must survive.</summary>
public sealed record DeleteCaregiverCommand(int CaregiverId) : IRequest;
