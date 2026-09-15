using CareConnect.DTOs.Clients;
using CareConnect.DTOs.Common;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Common;
using CareConnect.Queries.Security;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Clients.GetClients;

public sealed class GetClientsQueryHandler : IRequestHandler<GetClientsQuery, PagedResponseDto<ClientDto>>
{
    private static readonly string CountSql =
        SqlResourceLoader.Load(typeof(GetClientsQueryHandler), "GetClientsCountQuery.sql");

    private static readonly string DataSql =
        SqlResourceLoader.Load(typeof(GetClientsQueryHandler), "GetClientsQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetClientsQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponseDto<ClientDto>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        using var connection = _connectionFactory.CreateConnection();

        var requester = await RequesterResolver.ResolveAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        // Scopes rows to what this caller is actually allowed to see — never trusts the caller to
        // ask nicely. The role decides which branch of the SQL's scoping predicate applies: Admin
        // sees everything; a Client only ever matches their own row; a Caregiver only matches
        // clients they have an assignment with.
        var parameters = new
        {
            RequesterRole = requester.Role,
            RequesterClientId = requester.ClientId,
            RequesterCaregiverId = requester.CaregiverId,
            Search = string.IsNullOrWhiteSpace(request.Search) ? null : request.Search.Trim(),
            request.IsActive,
            Offset = (page - 1) * pageSize,
            PageSize = pageSize,
        };

        var totalRecords = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            CountSql, parameters, cancellationToken: cancellationToken));

        var data = await connection.QueryAsync<ClientDto>(new CommandDefinition(
            DataSql, parameters, cancellationToken: cancellationToken));

        return new PagedResponseDto<ClientDto>
        {
            Data = data.ToList(),
            CurrentPage = page,
            PageSize = pageSize,
            TotalRecords = totalRecords,
        };
    }
}
