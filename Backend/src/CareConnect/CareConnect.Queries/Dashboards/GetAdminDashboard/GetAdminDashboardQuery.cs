using CareConnect.DTOs.Dashboards;
using MediatR;

namespace CareConnect.Queries.Dashboards.GetAdminDashboard;

public sealed record GetAdminDashboardQuery(string RequestingAuth0UserId) : IRequest<AdminDashboardDto>;
