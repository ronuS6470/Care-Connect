using MediatR;

namespace CareConnect.Commands.Clients;

/// <summary>Soft-deletes (deactivates) a client — historical visit/invoice records must survive.</summary>
public sealed record DeleteClientCommand(int ClientId) : IRequest;
