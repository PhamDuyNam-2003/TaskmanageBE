using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BE.DTOs.Common;
using BE.DTOs.Tasks;
using BE.Helpers;
using BE.Services.Interfaces;

namespace BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Guid? projectId = null)
        {
            var userId = User.GetUserId();
            var result = await _taskService.GetAllAsync(userId, pageNumber, pageSize, projectId);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _taskService.GetByIdAsync(id);
            if (result == null) return NotFound(new ApiResponse<object> { Success = false, Message = "Task không tồn tại" });
            return Ok(new ApiResponse<object> { Success = true, Data = result });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTaskDto dto)
        {
            var userId = User.GetUserId();
            var result = await _taskService.CreateAsync(userId, dto);
            return Ok(new ApiResponse<object> { Success = true, Message = "Tạo task thành công", Data = result });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateTaskDto dto)
        {
            var result = await _taskService.UpdateAsync(id, dto);
            if (result == null) return NotFound(new ApiResponse<object> { Success = false, Message = "Task không tồn tại" });
            return Ok(new ApiResponse<object> { Success = true, Message = "Cập nhật task thành công", Data = result });
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, UpdateTaskStatusDto dto)
        {
            await _taskService.UpdateStatusAsync(id, dto.Status);
            return Ok(new ApiResponse<object> { Success = true, Message = "Cập nhật trạng thái thành công" });
        }

        [HttpPost("{id}/comments")]
        public async Task<IActionResult> AddComment(Guid id, CreateCommentDto dto)
        {
            var userId = User.GetUserId();
            await _taskService.AddCommentAsync(id, userId, dto.Content);
            return Ok(new ApiResponse<object> { Success = true, Message = "Thêm comment thành công" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _taskService.DeleteAsync(id);
            if (!result) return NotFound(new ApiResponse<object> { Success = false, Message = "Task không tồn tại" });
            return Ok(new ApiResponse<object> { Success = true, Message = "Xóa task thành công" });
        }

        [HttpGet("{id}/subtasks")]
        public async Task<IActionResult> GetSubTasks(Guid id)
        {
            var result = await _taskService.GetSubTasksAsync(id);
            return Ok(new ApiResponse<object> { Success = true, Data = result });
        }

        [HttpPost("{id}/subtasks")]
        public async Task<IActionResult> CreateSubTask(Guid id, CreateSubTaskDto dto)
        {
            var result = await _taskService.CreateSubTaskAsync(id, dto);
            return Ok(new ApiResponse<object> { Success = true, Data = result });
        }

        [HttpPut("subtasks/{subTaskId}")]
        public async Task<IActionResult> UpdateSubTask(Guid subTaskId, UpdateSubTaskDto dto)
        {
            var result = await _taskService.UpdateSubTaskAsync(subTaskId, dto);
            return Ok(new ApiResponse<object> { Success = true, Data = result });
        }

        [HttpDelete("subtasks/{subTaskId}")]
        public async Task<IActionResult> DeleteSubTask(Guid subTaskId)
        {
            await _taskService.DeleteSubTaskAsync(subTaskId);
            return Ok(new ApiResponse<object> { Success = true, Message = "Xóa subtask thành công" });
        }
    }
}
