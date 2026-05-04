using BE.Models;
using BE.Services.Interfaces;
using BE.DTOs; // Giả định sếp để DTO ở đây
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BE.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // GET: api/task
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleStr = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userId == null) return Unauthorized();

            Enum.TryParse(roleStr, out UserRole role);
            var tasks = await _taskService.GetAllTasksAsync(userId, role);
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
        {
            // 1. Lấy User ID trực tiếp từ Token (Claim)
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // 2. Map từ DTO sang Model thực tế
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = (TaskPriority)dto.Priority,
                CreatedBy = userId,       
                AssignedTo = userId,      
                Status = WorkStatus.Todo,
                CreatedAt = DateTime.UtcNow
            };

            var createdTask = await _taskService.CreateTaskAsync(task);
            return Ok(createdTask);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null) return NotFound(new { message = "Không tìm thấy task" });
            return Ok(task);
        }

        // PATCH: api/task/{id}/assign/{assigneeId}
        [HttpPatch("{id}/assign/{assigneeId}")]
        public async Task<IActionResult> Assign(string id, string assigneeId)
        {
            var managerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (managerId == null) return Unauthorized();

            var result = await _taskService.AssignTaskAsync(id, assigneeId, managerId);
            if (!result) return BadRequest(new { message = "Giao việc thất bại hoặc không đủ quyền" });

            return Ok(new { message = "Giao việc thành công" });
        }

        // PATCH: api/task/{id}/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] WorkStatus status)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var result = await _taskService.UpdateStatusAsync(id, status, userId);
            if (!result) return BadRequest(new { message = "Cập nhật thất bại" });

            return Ok(new { message = "Cập nhật trạng thái thành công" });
        }


        [HttpPatch("{id}/progress")]
        public async Task<IActionResult> UpdateProgress(string id, [FromBody] TaskUpdateProgressDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var success = await _taskService.UpdateStatusAsync(id, dto.Status, userId!);
            return success ? Ok(new { message = "Cập nhật thành công" }) : BadRequest("Không có quyền hoặc lỗi");
        }


        [HttpPost("{id}/comments")]
        public async Task<IActionResult> AddComment(string id, [FromBody] AddCommentDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var comment = new Comment
            {
                UserId = userId!,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };
            var success = await _taskService.AddCommentAsync(id, comment);
            return success ? Ok(comment) : BadRequest();
        }
    }
}