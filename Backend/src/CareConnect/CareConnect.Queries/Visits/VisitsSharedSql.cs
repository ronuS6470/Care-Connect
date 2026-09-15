using CareConnect.Queries.Common;

namespace CareConnect.Queries.Visits;

/// <summary>SQL used by more than one Visits call. Per-call SQL lives in that call's own folder.</summary>
internal static class VisitsSharedSql
{
    public static readonly string TasksByVisitId =
        SqlResourceLoader.Load(typeof(VisitsSharedSql), "GetVisitTasksByVisitIdQuery.sql");
}
