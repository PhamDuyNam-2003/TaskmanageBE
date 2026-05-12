using BE.DTOs.Common;
using BE.DTOs.Tasks;
using BE.Models.Enums;

namespace BE.Services.Interfaces
{
    public interface ITaskService
    {
        Task<PaginatedResponse<TaskDto>> GetAllAsync(
            Guid userId,
            int pageNumber,
            int pageSize,
            Guid? projectId = null);

        Task<TaskDto?> GetByIdAsync(Guid id);

        Task<TaskDto> CreateAsync(
            Guid createdById,
            CreateTaskDto dto);

        Task<TaskDto?> UpdateAsync(
            Guid id,
            UpdateTaskDto dto);

        Task<bool> DeleteAsync(Guid id);

        Task UpdateStatusAsync(
            Guid taskId,
            WorkStatus status);

        Task AddCommentAsync(
            Guid taskId,
            Guid userId,
            string content);

        // SubTask Management
        Task<IEnumerable<SubTaskDto>> GetSubTasksAsync(Guid taskId);

        Task<SubTaskDto?> GetSubTaskByIdAsync(Guid subTaskId);

        Task<SubTaskDto> CreateSubTaskAsync(
            Guid taskId,
            CreateSubTaskDto dto);

        Task<SubTaskDto?> UpdateSubTaskAsync(
            Guid subTaskId,
            UpdateSubTaskDto dto);

        Task<bool> DeleteSubTaskAsync(Guid subTaskId);
    }
}
