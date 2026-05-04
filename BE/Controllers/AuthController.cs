using BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static BE.DTOs.AuthDto;

namespace BE.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpPost("login")]
        public async Task<IActionResult> login(LoginDto dto) 
        {
            var result = await _authService.LoginAsync(dto);
            return result.Success ? Ok(result) : Unauthorized(result);
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            return Ok(new { message = "Bạn đã truy cập được vào vùng cấm!", user = User.Identity?.Name });
        }

    }
}
