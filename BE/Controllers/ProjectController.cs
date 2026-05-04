using BE.Models;
using BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BE.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var roleStr = User.FindFirst(ClaimTypes.Role)?.Value!;
            Enum.TryParse(roleStr, out UserRole role);

            var projects = await _projectService.GetAllProjectsAsync(userId, role);
            return Ok(projects);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Project project)
        {
            project.OwnerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var newProject = await _projectService.CreateProjectAsync(project);
            return Ok(newProject);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Project project)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var result = await _projectService.UpdateProjectAsync(id, project, userId);
            return result ? Ok(new { message = "Cập nhật thành công" }) : Forbid();
        }

        [HttpPost("{id}/members/{memberId}")]
        public async Task<IActionResult> AddMember(string id, string memberId)
        {
            var ownerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var result = await _projectService.AddMemberAsync(id, memberId, ownerId);
            return result ? Ok(new { message = "Đã thêm thành viên" }) : BadRequest();
        }

        [HttpDelete("{id}/members/{memberId}")]
        public async Task<IActionResult> RemoveMember(string id, string memberId)
        {
            var ownerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var result = await _projectService.RemoveMemberAsync(id, memberId, ownerId);
            return result ? Ok(new { message = "Đã xóa thành viên" }) : BadRequest();
        }
    }
}
