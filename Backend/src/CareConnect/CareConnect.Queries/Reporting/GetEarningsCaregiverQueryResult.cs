namespace CareConnect.Queries.Reporting;

/// <summary>Row shape for GetEarningsCaregiverQuery.sql.</summary>
internal sealed class EarningsCaregiverRow
{
    public int Id { get; init; }

    public decimal HourlyRate { get; init; }

    public string FullName { get; init; } = string.Empty;
}
