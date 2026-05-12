using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using BE.Data;
using BE.DTOs.Common;
using BE.DTOs.Projects;
using BE.Models;
using BE.Models.Enums;
using BE.Services.Interfaces;

namespace BE.Services.Implements
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProjectService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllAsync()
        {
            // Fallback for non-paginated call
            return await _context.Projects
                .ProjectTo<ProjectDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<PaginatedResponse<ProjectDto>> GetAllAsync(Guid userId, int pageNumber, int pageSize)
        {
            // LỌC: Chỉ lấy dự án mà user là chủ sở hữu hoặc là thành viên
            var query = _context.Projects
                .Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId))
                .OrderByDescending(x => x.CreatedAt);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ProjectDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PaginatedResponse<ProjectDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        // Overload to satisfy interface if needed (legacy)
        public async Task<PaginatedResponse<ProjectDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Projects.OrderByDescending(x => x.CreatedAt);
            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber-1)*pageSize).Take(pageSize).ProjectTo<ProjectDto>(_mapper.ConfigurationProvider).ToListAsync();
            return new PaginatedResponse<ProjectDto> { Items = items, TotalCount = totalCount, PageNumber = pageNumber, PageSize = pageSize };
        }

        public async Task<ProjectDto?> GetByIdAsync(Guid id)
        {
            return await _context.Projects
                .Where(x => x.Id == id)
                .ProjectTo<ProjectDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<ProjectDto> CreateAsync(Guid ownerId, CreateProjectDto dto)
        {
            var project = new Project
            {
                Name = dto.Name,
                Description = dto.Description,
                OwnerId = ownerId
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            // Tự động thêm Owner vào danh sách Members với role Owner
            var member = new ProjectMember
            {
                ProjectId = project.Id,
                UserId = ownerId,
                Role = ProjectRole.Owner
            };

            _context.ProjectMembers.Add(member);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(project.Id) ?? _mapper.Map<ProjectDto>(project);
        }

        public async Task<ProjectDto?> UpdateAsync(Guid id, Guid userId, UpdateProjectDto dto)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == id);
            if (project == null) return null;

            // Check authorization - only Owner or Manager can update
            var userRole = await _context.ProjectMembers
                .Where(x => x.ProjectId == id && x.UserId == userId)
                .Select(x => x.Role)
                .FirstOrDefaultAsync();

            if (project.OwnerId != userId && userRole != ProjectRole.Manager && userRole != ProjectRole.Owner)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền cập nhật project này");
            }

            project.Name = dto.Name;
            project.Description = dto.Description;
            project.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == id);
            if (project == null) return false;

            if (project.OwnerId != userId)
            {
                throw new UnauthorizedAccessException("Chỉ chủ sở hữu mới có quyền xóa project");
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task AddMemberAsync(Guid projectId, Guid userId, AddProjectMemberDto dto)
        {
            var project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(x => x.Id == projectId);
            if (project == null) throw new Exception("Project không tồn tại");

            var requesterRole = await _context.ProjectMembers
                .Where(x => x.ProjectId == projectId && x.UserId == userId)
                .Select(x => x.Role)
                .FirstOrDefaultAsync();

            if (project.OwnerId != userId && requesterRole != ProjectRole.Manager && requesterRole != ProjectRole.Owner)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền thêm thành viên");
            }

            if (project.Members.Any(m => m.UserId == dto.UserId))
            {
                throw new Exception("User đã là thành viên của project");
            }

            var member = new ProjectMember
            {
                ProjectId = projectId,
                UserId = dto.UserId,
                Role = (ProjectRole)dto.Role
            };

            _context.ProjectMembers.Add(member);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveMemberAsync(Guid projectId, Guid memberId, Guid requestingUserId)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == projectId);
            if (project == null) return false;

            var requesterRole = await _context.ProjectMembers
                .Where(x => x.ProjectId == projectId && x.UserId == requestingUserId)
                .Select(x => x.Role)
                .FirstOrDefaultAsync();

            if (project.OwnerId != requestingUserId && requesterRole != ProjectRole.Manager && requesterRole != ProjectRole.Owner)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền xóa thành viên");
            }

            if (project.OwnerId == memberId) throw new Exception("Không thể xóa chủ sở hữu");

            var member = await _context.ProjectMembers.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.UserId == memberId);
            if (member == null) return false;

            _context.ProjectMembers.Remove(member);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
