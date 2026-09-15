using CareConnect.DTOs.Dashboards;
using MediatR;

namespace CareConnect.Queries.Dashboards;

/// <summary>Always the authenticated caller's own dashboard — no clientId parameter.</summary>
public sealed record GetClientDashboardQuery(string RequestingAuth0UserId) : IRequest<ClientDashboardDto>;
