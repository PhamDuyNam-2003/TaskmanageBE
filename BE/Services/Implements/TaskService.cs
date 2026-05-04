using BE.Models;
using BE.Services.Interfaces;
using MongoDB.Driver;

namespace BE.Services.Implements
{
    public class TaskService : ITaskService

    {
        private readonly IMongoCollection<TaskItem> _tasks;
        private readonly IMongoCollection<User> _users;


        public TaskService(IMongoDatabase database)
        {
            _tasks = database.GetCollection<TaskItem>("Tasks");
            _users = database.GetCollection<User>("Users");
        }

        public async Task<List<TaskItem>> GetAllTasksAsync(string userId, UserRole role)
        {
            if (role == UserRole.Admin || role == UserRole.Manager)
            {
                return await _tasks.Find(t => t.IsDeleted == false).ToListAsync();
            }

            return await _tasks.Find(t =>
                t.IsDeleted == false && (
                    t.CreatedBy == userId ||
                    t.AssignedTo == userId ||
                    (t.CollaboratorIds != null && t.CollaboratorIds.Contains(userId))
                )
            ).ToListAsync();
        }

        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            task.CreatedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;
            task.IsDeleted = false;

            // Khởi tạo các list để tránh lỗi null sau này
            task.CollaboratorIds ??= new List<string>();
            task.Comments ??= new List<Comment>();

            await _tasks.InsertOneAsync(task);
            return task;
        }

        public async Task<bool> AssignTaskAsync(string taskId, string assigneeId, string managerId)
        {
            var task = await _tasks.Find(t => t.Id == taskId).FirstOrDefaultAsync();
            if (task == null) return false;

            var manager = await _users.Find(u => u.Id == managerId).FirstOrDefaultAsync();
            if (manager == null) return false;

            // Nếu là Member thường thì chỉ được assign task do chính mình tạo
            if (manager.Role == UserRole.Member && task.CreatedBy != managerId) return false;

            var update = Builders<TaskItem>.Update
                .Set(t => t.AssignedTo, assigneeId)
                .Set(t => t.UpdatedAt, DateTime.UtcNow);

            var result = await _tasks.UpdateOneAsync(t => t.Id == taskId, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateStatusAsync(string taskId, WorkStatus status, string userId)
        {
            var filter = Builders<TaskItem>.Filter.And(
                Builders<TaskItem>.Filter.Eq(t => t.Id, taskId),
                Builders<TaskItem>.Filter.Or(
                    Builders<TaskItem>.Filter.Eq(t => t.AssignedTo, userId),
                    Builders<TaskItem>.Filter.Eq(t => t.CreatedBy, userId)
                )
            );

            var update = Builders<TaskItem>.Update
                .Set(t => t.Status, status)
                .Set(t => t.UpdatedAt, DateTime.UtcNow);

            if (status == WorkStatus.Done)
                update = update.Set(t => t.Progress, 100);

            var result = await _tasks.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> AddCommentAsync(string taskId, Comment comment)
        {
            comment.CreatedAt = DateTime.UtcNow;

            var update = Builders<TaskItem>.Update.Push(t => t.Comments, comment);
            var result = await _tasks.UpdateOneAsync(t => t.Id == taskId, update);

            return result.ModifiedCount > 0;
        }

        public async Task<TaskItem?> GetTaskByIdAsync(string id) =>
            await _tasks.Find(t => t.Id == id).FirstOrDefaultAsync();

        public async Task<bool> UpdateTaskAsync(string taskId, TaskItem task, string userId, UserRole role)
        {
            var existingTask = await _tasks.Find(t => t.Id == taskId).FirstOrDefaultAsync();
            if (existingTask == null) return false;

            if (role == UserRole.Member && existingTask.CreatedBy != userId) return false;

            var update = Builders<TaskItem>.Update
                .Set(t => t.Title, task.Title)
                .Set(t => t.Description, task.Description)
                .Set(t => t.Priority, task.Priority)
                .Set(t => t.DueDate, task.DueDate)
                .Set(t => t.ProjectId, task.ProjectId)
                .Set(t => t.UpdatedAt, DateTime.UtcNow);

            var result = await _tasks.UpdateOneAsync(t => t.Id == taskId, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteTaskAsync(string taskId, string userId, UserRole role)
        {
            var task = await _tasks.Find(t => t.Id == taskId).FirstOrDefaultAsync();
            if (task == null) return false;

            if (role == UserRole.Member && task.CreatedBy != userId) return false;

            var update = Builders<TaskItem>.Update
                .Set(t => t.IsDeleted, true)
                .Set(t => t.UpdatedAt, DateTime.UtcNow);

            var result = await _tasks.UpdateOneAsync(t => t.Id == taskId, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> AddCollaboratorAsync(string taskId, string collaboratorId)
        {
            var userExists = await _users.Find(u => u.Id == collaboratorId).AnyAsync();
            if (!userExists) return false;

            var update = Builders<TaskItem>.Update
                .AddToSet(t => t.CollaboratorIds, collaboratorId)
                .Set(t => t.UpdatedAt, DateTime.UtcNow);

            var result = await _tasks.UpdateOneAsync(t => t.Id == taskId, update);
            return result.ModifiedCount > 0;
        }
    }
}