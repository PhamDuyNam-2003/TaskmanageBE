using BE.DTOs.Auth;

namespace BE.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(
            RegisterRequestDto dto);

        Task<AuthResponseDto> RefreshTokenAsync(
             string refreshToken);

        Task<AuthResponseDto> LoginAsync(
            LoginRequestDto dto);

        string GenerateAccessToken(Models.User user);

        string GenerateRefreshToken();
        Task LogoutAsync(Guid userId);
    }
}