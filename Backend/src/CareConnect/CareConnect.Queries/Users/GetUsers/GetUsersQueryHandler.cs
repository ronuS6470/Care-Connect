using CareConnect.DTOs.Common;
using CareConnect.DTOs.Enums;
using CareConnect.DTOs.Users;
using CareConnect.Infrastructure.Data;
using CareConnect.Infrastructure.Errors;
using CareConnect.Queries.Common;
using CareConnect.Queries.Security;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Users.GetUsers;

public sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResponseDto<UserDto>>
{
    private static readonly string CountSql =
        SqlResourceLoader.Load(typeof(GetUsersQueryHandler), "GetUsersCountQuery.sql");

    private static readonly string DataSql =
        SqlResourceLoader.Load(typeof(GetUsersQueryHandler), "GetUsersQuery.sql");

    private readonly IDbConnectionFactory _connectionFactory;

    public GetUsersQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponseDto<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        using var connection = _connectionFactory.CreateConnection();

        // Checked here as well as on the controller: seeing every account in the system is
        // Admin-only, and this layer never assumes the HTTP layer got it right.
        var requester = await RequesterResolver.ResolveAsync(connection, request.RequestingAuth0UserId, cancellationToken);

        if (requester.Role != UserRole.Admin)
        {
            throw new ForbiddenException("Only Admin may list users.");
        }

        var parameters = new
        {
            Search = string.IsNullOrWhiteSpace(request.Search) ? null : request.Search.Trim(),
            request.Role,
            request.IsActive,
            Offset = (page - 1) * pageSize,
            PageSize = pageSize,
        };

        var totalRecords = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            CountSql, parameters, cancellationToken: cancellationToken));

        var data = await connection.QueryAsync<UserDto>(new CommandDefinition(
            DataSql, parameters, cancellationToken: cancellationToken));

        return new PagedResponseDto<UserDto>
        {
            Data = data.ToList(),
            CurrentPage = page,
            PageSize = pageSize,
            TotalRecords = totalRecords,
        };
    }
}
