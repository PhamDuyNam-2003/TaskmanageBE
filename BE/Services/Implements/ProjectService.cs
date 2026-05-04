using BE.Models;
using BE.Services.Interfaces;
using MongoDB.Driver;

namespace BE.Services.Implements
{
    public class ProjectService : IProjectService 
    {
        private readonly IMongoCollection<Project> _projects;
        private readonly IMongoCollection<User> _users;

        public ProjectService(IMongoDatabase database)
        {
            _projects = database.GetCollection<Project>("Projects");
            _users = database.GetCollection<User>("Users");
        }
        public async Task<List<Project>> GetAllProjectsAsync(string userId, UserRole role)
        {
            if (role == UserRole.Admin) return await _projects.Find(_ => true).ToListAsync();

            return await _projects.Find(p =>
                p.OwnerId == userId || p.MemberIds.Contains(userId)
            ).ToListAsync();
        }
        public async Task<Project> CreateProjectAsync(Project project)
        {
            project.CreatedAt = DateTime.UtcNow;
            if (!project.MemberIds.Contains(project.OwnerId))
            {
                project.MemberIds.Add(project.OwnerId);
            }
            await _projects.InsertOneAsync(project);
            return project;
        }
        public async Task<bool> AddMemberAsync(string projectId, string memberId, string ownerId)
        {
            var userExists = await _users.Find(u => u.Id == memberId).AnyAsync();
            if (!userExists) return false;

            var filter = Builders<Project>.Filter.And(
                Builders<Project>.Filter.Eq(p => p.Id, projectId),
                Builders<Project>.Filter.Eq(p => p.OwnerId, ownerId) 
            );

            var update = Builders<Project>.Update.AddToSet(p => p.MemberIds, memberId);
            var result = await _projects.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
        public async Task<bool> DeleteProjectAsync(string id, string userId)
        {
            var result = await _projects.DeleteOneAsync(p => p.Id == id && p.OwnerId == userId);
            return result.DeletedCount > 0;
        }

        public async Task<Project?> GetProjectByIdAsync(string id) =>
            await _projects.Find(p => p.Id == id).FirstOrDefaultAsync();
        public async Task<bool> UpdateProjectAsync(string id, Project project, string userId)
        {
            var filter = Builders<Project>.Filter.And(
                Builders<Project>.Filter.Eq(p => p.Id, id),
                Builders<Project>.Filter.Eq(p => p.OwnerId, userId)
            );

            var update = Builders<Project>.Update
                .Set(p => p.Name, project.Name)
                .Set(p => p.Description, project.Description)
                .Set(p => p.UpdatedAt, DateTime.UtcNow); 

            var result = await _projects.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
        public async Task<bool> RemoveMemberAsync(string projectId, string memberId, string ownerId)
        {
            if (memberId == ownerId) return false;

            var filter = Builders<Project>.Filter.And(
                Builders<Project>.Filter.Eq(p => p.Id, projectId),
                Builders<Project>.Filter.Eq(p => p.OwnerId, ownerId)
            );

            var update = Builders<Project>.Update.Pull(p => p.MemberIds, memberId);

            var result = await _projects.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
    }
}
