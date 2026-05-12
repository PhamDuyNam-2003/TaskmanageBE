using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BE.DTOs.Common;
using BE.DTOs.Projects;
using BE.Helpers;
using BE.Services.Interfaces;

namespace BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = User.GetUserId();
            var result = await _projectService.GetAllAsync(userId, pageNumber, pageSize);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _projectService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Project không tồn tại"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = result
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProjectDto dto)
        {
            var userId = User.GetUserId();
            var result = await _projectService.CreateAsync(userId, dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Tạo project thành công",
                Data = result
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateProjectDto dto)
        {
            var userId = User.GetUserId();
            try
            {
                var result = await _projectService.UpdateAsync(id, userId, dto);
                if (result == null)
                {
                    return NotFound(new ApiResponse<object> { Success = false, Message = "Project không tồn tại" });
                }
                return Ok(new ApiResponse<object> { Success = true, Message = "Cập nhật thành công", Data = result });
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.GetUserId();
            try
            {
                var result = await _projectService.DeleteAsync(id, userId);
                if (!result) return NotFound(new ApiResponse<object> { Success = false, Message = "Project không tồn tại" });
                return Ok(new ApiResponse<object> { Success = true, Message = "Xóa project thành công" });
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpPost("{id}/members")]
        public async Task<IActionResult> AddMember(Guid id, AddProjectMemberDto dto)
        {
            var userId = User.GetUserId();
            try
            {
                await _projectService.AddMemberAsync(id, userId, dto);
                return Ok(new ApiResponse<object> { Success = true, Message = "Thêm member thành công" });
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (Exception ex) { return BadRequest(new ApiResponse<object> { Success = false, Message = ex.Message }); }
        }

        [HttpDelete("{id}/members/{memberId}")]
        public async Task<IActionResult> RemoveMember(Guid id, Guid memberId)
        {
            var userId = User.GetUserId();
            try
            {
                var result = await _projectService.RemoveMemberAsync(id, memberId, userId);
                if (!result) return NotFound(new ApiResponse<object> { Success = false, Message = "Thành viên không tồn tại" });
                return Ok(new ApiResponse<object> { Success = true, Message = "Xóa member thành công" });
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (Exception ex) { return BadRequest(new ApiResponse<object> { Success = false, Message = ex.Message }); }
        }
    }
}
