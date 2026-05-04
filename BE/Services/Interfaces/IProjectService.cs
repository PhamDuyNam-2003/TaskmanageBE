using BE.Models;

namespace BE.Services.Interfaces
{
    public interface IProjectService
    {
        Task<List<Project>> GetAllProjectsAsync(string userId, UserRole role);
        Task<Project?> GetProjectByIdAsync(string id);
        Task<Project> CreateProjectAsync(Project project);
        Task<bool> UpdateProjectAsync(string id, Project project, string userId);
        Task<bool> DeleteProjectAsync(string id, string userId);
        Task<bool> AddMemberAsync(string projectId, string memberId, string ownerId);
        Task<bool> RemoveMemberAsync(string projectId, string memberId, string ownerId);
    }
}
