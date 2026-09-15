namespace CareConnect.Queries.Assignments.GetAssignmentById;

/// <summary>Row shape for GetAssignmentOwnerQuery.sql.</summary>
internal sealed class AssignmentOwnerRow
{
    public int CaregiverId { get; init; }

    public int ClientId { get; init; }
}
