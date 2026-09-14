using MediatR;

namespace CareConnect.Commands.Caregivers;

public sealed record DeactivateCaregiverCommand(int CaregiverId) : IRequest;
