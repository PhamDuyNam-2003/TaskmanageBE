using AutoMapper;
using AutoMapper.QueryableExtensions;
using BE.Data;
using BE.DTOs.Common;
using BE.DTOs.Tasks;
using BE.Models;
using BE.Models.Enums;
using BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BE.Services.Implements
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TaskService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<TaskDto>> GetAllAsync(
            Guid userId,
            int pageNumber,
            int pageSize,
            Guid? projectId = null)
        {
            // LỌC: Chỉ lấy task thuộc project mà user tham gia HOẶC task được giao trực tiếp cho user
            var query = _context.TaskItems
                .Where(t => !t.IsDeleted)
                .Where(t => (t.ProjectId != null && t.Project.Members.Any(m => m.UserId == userId))
                         || t.AssignedToId == userId
                         || t.CreatedById == userId)
                .AsQueryable();

            if (projectId.HasValue)
            {
                query = query.Where(x => x.ProjectId == projectId);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<TaskDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PaginatedResponse<TaskDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<TaskDto?> GetByIdAsync(Guid id)
        {
            return await _context.TaskItems
                .Where(x => x.Id == id && !x.IsDeleted)
                .ProjectTo<TaskDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<TaskDto> CreateAsync(Guid createdById, CreateTaskDto dto)
        {
            var assignedUser = await _context.Users.AnyAsync(x => x.Id == dto.AssignedToId);
            if (!assignedUser) throw new Exception("Assigned user không tồn tại");

            var task = new TaskItem
            {
                ProjectId = dto.ProjectId,
                Title = dto.Title,
                Description = dto.Description,
                AssignedToId = dto.AssignedToId,
                CreatedById = createdById,
                Priority = dto.Priority,
                Progress = 0,
                DueDate = dto.DueDate?.ToUniversalTime()
            };

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(task.Id) ?? _mapper.Map<TaskDto>(task);
        }

        public async Task<TaskDto?> UpdateAsync(Guid id, UpdateTaskDto dto)
        {
            var task = await _context.TaskItems.FirstOrDefaultAsync(x => x.Id == id);
            if (task == null) return null;

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Status = dto.Status;
            task.Priority = dto.Priority;
            task.Progress = dto.Progress;
            task.DueDate = dto.DueDate?.ToUniversalTime();
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var task = await _context.TaskItems.FirstOrDefaultAsync(x => x.Id == id);
            if (task == null) return false;

            task.IsDeleted = true;
            task.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task UpdateStatusAsync(Guid taskId, WorkStatus status)
        {
            var task = await _context.TaskItems.FirstOrDefaultAsync(x => x.Id == taskId);
            if (task == null) throw new Exception("Task không tồn tại");

            task.Status = status;
            task.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task AddCommentAsync(Guid taskId, Guid userId, string content)
        {
            var comment = new Comment
            {
                TaskItemId = taskId,
                UserId = userId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<SubTaskDto>> GetSubTasksAsync(Guid taskId)
        {
            return await _context.SubTasks
                .Where(x => x.TaskItemId == taskId)
                .OrderBy(x => x.CreatedAt)
                .ProjectTo<SubTaskDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<SubTaskDto?> GetSubTaskByIdAsync(Guid subTaskId)
        {
            return await _context.SubTasks
                .Where(x => x.Id == subTaskId)
                .ProjectTo<SubTaskDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<SubTaskDto> CreateSubTaskAsync(Guid taskId, CreateSubTaskDto dto)
        {
            var subTask = new SubTask { TaskItemId = taskId, Title = dto.Title };
            _context.SubTasks.Add(subTask);
            await _context.SaveChangesAsync();
            return _mapper.Map<SubTaskDto>(subTask);
        }

        public async Task<SubTaskDto?> UpdateSubTaskAsync(Guid subTaskId, UpdateSubTaskDto dto)
        {
            var subTask = await _context.SubTasks.FirstOrDefaultAsync(x => x.Id == subTaskId);
            if (subTask == null) return null;

            subTask.Title = dto.Title;
            subTask.IsDone = dto.IsDone;
            await _context.SaveChangesAsync();
            return _mapper.Map<SubTaskDto>(subTask);
        }

        public async Task<bool> DeleteSubTaskAsync(Guid subTaskId)
        {
            var subTask = await _context.SubTasks.FirstOrDefaultAsync(x => x.Id == subTaskId);
            if (subTask == null) return false;
            _context.SubTasks.Remove(subTask);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
