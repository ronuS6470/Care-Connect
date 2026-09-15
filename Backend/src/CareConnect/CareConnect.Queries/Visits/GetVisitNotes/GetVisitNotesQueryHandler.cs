using CareConnect.DTOs.Visits;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using CareConnect.Queries.Security;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Visits.GetVisitNotes;

public sealed class GetVisitNotesQueryHandler : IRequestHandler<GetVisitNotesQuery, IReadOnlyList<VisitNoteDto>>
{
    private static readonly string Sql =
        SqlResourceLoader.Load(typeof(GetVisitNotesQueryHandler), "GetVisitNotesQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetVisitNotesQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<VisitNoteDto>> Handle(GetVisitNotesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var requester = await RequesterResolver.ResolveAsync(connection, request.RequestingAuth0UserId, cancellationToken);
        await VisitAccessResolver.EnsureCanViewVisitAsync(connection, request.VisitId, requester, cancellationToken);

        var notes = await connection.QueryAsync<VisitNoteDto>(new CommandDefinition(
            Sql,
            new { request.VisitId },
            cancellationToken: cancellationToken));

        return notes.ToList();
    }
}
