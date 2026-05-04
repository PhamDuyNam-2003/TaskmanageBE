using BE.Models;

namespace BE.Services.Interfaces
{
    public interface ITaskService
    { //get đb
        Task<List<TaskItem>> GetAllTasksAsync(string userId, UserRole role);
        Task<TaskItem?> GetTaskByIdAsync(string id);

        // CRUD 
        Task<TaskItem> CreateTaskAsync(TaskItem task);
        Task<bool> UpdateTaskAsync(string taskId, TaskItem task, string userId, UserRole role);
        Task<bool> DeleteTaskAsync(string taskId, string userId, UserRole role);

        // chi viec, tham du
        Task<bool> AssignTaskAsync(string taskId, string assigneeId, string managerId);
        Task<bool> AddCollaboratorAsync(string taskId, string collaboratorId);

        // toc do lam va xcmt
        Task<bool> UpdateStatusAsync(string taskId, WorkStatus status, string userId);
        Task<bool> AddCommentAsync(string taskId, Comment comment);
    }
}
