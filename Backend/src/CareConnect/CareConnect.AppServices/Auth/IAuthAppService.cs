using CareConnect.DTOs.Auth;

namespace CareConnect.AppServices.Auth;

public interface IAuthAppService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto credentials, CancellationToken cancellationToken);
}
