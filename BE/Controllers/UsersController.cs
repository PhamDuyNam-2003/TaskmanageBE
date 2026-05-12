using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using BE.DTOs.Common;
using BE.DTOs.Users;
using BE.Services.Interfaces;

namespace BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;


        public UsersController(
            IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _userService.GetAllAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,

                Data = result
            });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            Guid id)
        {
            var result =
                await _userService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,

                    Message = "User không tồn tại"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,

                Data = result
            });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            Guid id,
            UpdateUserDto dto)
        {
            var result =
                await _userService.UpdateAsync(id, dto);

            if (result == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,

                    Message = "User không tồn tại"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,

                Message = "Cập nhật thành công",

                Data = result
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            Guid id)
        {
            var result =
                await _userService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,

                    Message = "User không tồn tại"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,

                Message = "Xóa thành công"
            });
        }
    }
}