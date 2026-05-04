using BE.DTOs;
using BE.Models;
using static BE.DTOs.AuthDto;

namespace BE.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}