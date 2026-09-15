using CareConnect.DTOs.Dashboards;
using MediatR;

namespace CareConnect.Queries.Dashboards;

/// <summary>Always the authenticated caller's own dashboard — no caregiverId parameter.</summary>
public sealed record GetCaregiverDashboardQuery(string RequestingAuth0UserId) : IRequest<CaregiverDashboardDto>;
