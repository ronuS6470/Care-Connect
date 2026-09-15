using CareConnect.DTOs.Visits;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Security;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Visits.GetVisitTasks;

/// <summary>Its SQL is shared with GetVisitById, so it lives at the feature root — see VisitsSharedSql.</summary>
public sealed class GetVisitTasksQueryHandler : IRequestHandler<GetVisitTasksQuery, IReadOnlyList<VisitTaskDto>>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetVisitTasksQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<VisitTaskDto>> Handle(GetVisitTasksQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var requester = await RequesterResolver.ResolveAsync(connection, request.RequestingAuth0UserId, cancellationToken);
        await VisitAccessResolver.EnsureCanViewVisitAsync(connection, request.VisitId, requester, cancellationToken);

        var tasks = await connection.QueryAsync<VisitTaskDto>(new CommandDefinition(
            VisitsSharedSql.TasksByVisitId,
            new { request.VisitId },
            cancellationToken: cancellationToken));

        return tasks.ToList();
    }
}
