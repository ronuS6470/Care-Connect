using CareConnect.DTOs.Auth;

namespace CareConnect.AppServices.Auth;

public interface IAuthAppService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto credentials, CancellationToken cancellationToken);

    /// <summary>Changes the caller's own password. The target is never a parameter.</summary>
    Task ChangePasswordAsync(ChangePasswordDto dto, CancellationToken cancellationToken);
}
