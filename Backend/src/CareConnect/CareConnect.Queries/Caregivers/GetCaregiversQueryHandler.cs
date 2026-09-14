using CareConnect.DTOs.Caregivers;
using CareConnect.DTOs.Common;
using CareConnect.Infrastructure.Data;
using Dapper;
using MediatR;

namespace CareConnect.Queries.Caregivers;

public sealed class GetCaregiversQueryHandler : IRequestHandler<GetCaregiversQuery, PagedResponseDto<CaregiverDto>>
{
    private const string CountSql = "SELECT COUNT(*) FROM Caregivers;";

    private const string DataSql = """
        SELECT c.Id, c.UserId, u.FirstName + ' ' + u.LastName AS FullName, u.Email, u.PhoneNumber,
               c.LicenseNumber, c.HourlyRate, c.HireDate, c.DateOfBirth, c.YearsOfExperience, c.IsActive
        FROM Caregivers c
        INNER JOIN Users u ON u.Id = c.UserId
        ORDER BY c.Id
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        """;

    private readonly IDbConnectionFactory _connectionFactory;

    public GetCaregiversQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponseDto<CaregiverDto>> Handle(GetCaregiversQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        using var connection = _connectionFactory.CreateConnection();

        var totalRecords = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(CountSql, cancellationToken: cancellationToken));

        var data = await connection.QueryAsync<CaregiverDto>(new CommandDefinition(
            DataSql,
            new { Offset = (page - 1) * pageSize, PageSize = pageSize },
            cancellationToken: cancellationToken));

        return new PagedResponseDto<CaregiverDto>
        {
            Data = data.ToList(),
            CurrentPage = page,
            PageSize = pageSize,
            TotalRecords = totalRecords,
        };
    }
}
