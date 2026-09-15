using CareConnect.Infrastructure.Errors;
using Microsoft.AspNetCore.Http;

namespace CareConnect.AppServices.Security;

public sealed class CurrentUserAccessor : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string Auth0UserId =>
        _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value
            ?? throw new ForbiddenException("Token is missing a subject claim.");
}
