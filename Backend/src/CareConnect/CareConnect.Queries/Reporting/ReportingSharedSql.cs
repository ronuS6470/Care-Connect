using CareConnect.Queries.Common;

namespace CareConnect.Queries.Reporting;

/// <summary>SQL used by more than one Reporting call. Per-call SQL lives in that call's own folder.</summary>
internal static class ReportingSharedSql
{
    public static readonly string Caregiver =
        SqlResourceLoader.Load(typeof(ReportingSharedSql), "GetEarningsCaregiverQuery.sql");

    public static readonly string WorkedVisits =
        SqlResourceLoader.Load(typeof(ReportingSharedSql), "GetCaregiverWorkedVisitsQuery.sql");
}
