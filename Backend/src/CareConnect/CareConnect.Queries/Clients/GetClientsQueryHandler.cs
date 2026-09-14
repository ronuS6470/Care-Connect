using CareConnect.DTOs.Clients;
using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using CareConnect.Infrastructure.Data;
using CareConnect.Queries.Security;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Clients;

public sealed class GetClientsQueryHandler : IRequestHandler<GetClientsQuery, PagedResponseDto<ClientDto>>
{
    private const string SelectColumns = """
        c.Id, c.UserId, u.FirstName + ' ' + u.LastName AS FullName, u.Email, u.PhoneNumber,
        c.AddressLine1, c.AddressLine2, c.City, c.State, c.PostalCode,
        c.EmergencyContactName, c.EmergencyContactPhone, c.IsActive
        """;

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
        // ask nicely. Admin sees everything; a Client only ever matches their own row; a Caregiver
        // only matches clients they have an assignment with.
        var authFilterSql = requester.Role switch
        {
            UserRole.Admin => "1 = 1",
            UserRole.Client => "c.Id = @RequesterClientId",
            UserRole.Caregiver => "c.Id IN (SELECT ClientId FROM CaregiverAssignments WHERE CaregiverId = @RequesterCaregiverId)",
            _ => "1 = 0",
        };

        var whereSql = $"""
            WHERE ({authFilterSql})
              AND (@Search IS NULL OR u.FirstName LIKE '%' + @Search + '%' OR u.LastName LIKE '%' + @Search + '%' OR u.Email LIKE '%' + @Search + '%')
              AND (@IsActive IS NULL OR c.IsActive = @IsActive)
            """;

        var countSql = $"""
            SELECT COUNT(*)
            FROM Clients c
            INNER JOIN Users u ON u.Id = c.UserId
            {whereSql};
            """;

        var dataSql = $"""
            SELECT {SelectColumns}
            FROM Clients c
            INNER JOIN Users u ON u.Id = c.UserId
            {whereSql}
            ORDER BY c.Id
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        var parameters = new
        {
            RequesterClientId = requester.ClientId,
            RequesterCaregiverId = requester.CaregiverId,
            Search = string.IsNullOrWhiteSpace(request.Search) ? null : request.Search.Trim(),
            request.IsActive,
            Offset = (page - 1) * pageSize,
            PageSize = pageSize,
        };

        var totalRecords = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            countSql, parameters, cancellationToken: cancellationToken));

        var data = await connection.QueryAsync<ClientDto>(new CommandDefinition(
            dataSql, parameters, cancellationToken: cancellationToken));

        return new PagedResponseDto<ClientDto>
        {
            Data = data.ToList(),
            CurrentPage = page,
            PageSize = pageSize,
            TotalRecords = totalRecords,
        };
    }
}
