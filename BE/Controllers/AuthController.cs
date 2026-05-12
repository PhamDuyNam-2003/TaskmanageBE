using BE.DTOs.Auth;
using BE.DTOs.Common;
using BE.Helpers;
using BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;


        public AuthController(
            IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterRequestDto dto)
        {
            var result =
                await _authService.RegisterAsync(dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,

                Message = "Đăng ký thành công",

                Data = result
            });
        }

            [Authorize]
            [HttpPost("logout")]
            public async Task<IActionResult> Logout()
            {
                var userId = User.GetUserId();

                await _authService.LogoutAsync(userId);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Logout thành công"
                });
            }


        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginRequestDto dto)
        {
            var result =
                await _authService.LoginAsync(dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,

                Message = "Đăng nhập thành công",

                Data = result
            });
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(
                RefreshTokenRequestDto dto)
        {
            var result =
                await _authService
                    .RefreshTokenAsync(
                        dto.RefreshToken);

            return Ok(new ApiResponse<object>
            {
                Success = true,

                Message = "Refresh token thành công",

                Data = result
            });
        }

    }

}