using MediatR;

namespace CareConnect.Commands.CareTasks;

/// <summary>
/// Removes a care task template — or deactivates it, if VisitTask rows already reference it and a
/// physical delete would break historical visit records.
/// </summary>
public sealed record DeleteCareTaskCommand(int CareTaskId) : IRequest;
