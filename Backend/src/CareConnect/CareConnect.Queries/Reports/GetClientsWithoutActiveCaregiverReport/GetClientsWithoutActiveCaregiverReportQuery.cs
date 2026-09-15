using CareConnect.DTOs.Reports;
using MediatR;

namespace CareConnect.Queries.Reports.GetClientsWithoutActiveCaregiverReport;

public sealed record GetClientsWithoutActiveCaregiverReportQuery(string RequestingAuth0UserId)
    : IRequest<IReadOnlyList<ClientWithoutActiveCaregiverDto>>;
