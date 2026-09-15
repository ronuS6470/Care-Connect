namespace CareConnect.Queries.Reports.GetTopCaregiversByHoursReport;

/// <summary>Row shape for GetTopCaregiversByHoursReportQuery.sql — Rank is assigned in the handler.</summary>
internal sealed class TopCaregiverByHoursRow
{
    public int CaregiverId { get; init; }

    public string CaregiverFullName { get; init; } = string.Empty;

    public decimal TotalHoursWorked { get; init; }
}
