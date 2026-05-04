using BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BE.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) => _userService = userService;

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var user = await _userService.GetByIdAsync(userId);
            return Ok(new { user.Id, user.Username, user.Email, user.Role });
        }

        [HttpPatch("fcm-token")]
        public async Task<IActionResult> UpdateToken([FromBody] string token)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            await _userService.UpdateFcmTokenAsync(userId, token);
            return Ok(new { message = "Token updated" });
        }
    }
}
