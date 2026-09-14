using MediatR;

namespace CareConnect.Commands.Caregivers;

public sealed record ActivateCaregiverCommand(int CaregiverId) : IRequest;
