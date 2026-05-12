using BE.DTOs.Common;
using BE.DTOs.Projects;

namespace BE.Services.Interfaces
{
    public interface IProjectService
    {
        // Lọc dự án theo UserId
        Task<PaginatedResponse<ProjectDto>> GetAllAsync(
            Guid userId,
            int pageNumber,
            int pageSize);

        Task<ProjectDto?> GetByIdAsync(Guid id);

        Task<ProjectDto> CreateAsync(
            Guid ownerId,
            CreateProjectDto dto);

        Task<ProjectDto?> UpdateAsync(
            Guid id,
            Guid userId,
            UpdateProjectDto dto);

        Task<bool> DeleteAsync(Guid id, Guid userId);

        Task AddMemberAsync(
            Guid projectId,
            Guid userId,
            AddProjectMemberDto dto);

        Task<bool> RemoveMemberAsync(
            Guid projectId,
            Guid memberId,
            Guid requestingUserId);
    }
}
