using BE.Models.Enums;

namespace BE.Models
{
    public class User
    {
        public Guid Id { get; set; }
            = Guid.NewGuid();

        public string Username { get; set; }
            = null!;

        public string Email { get; set; }
            = null!;

        public string PasswordHash { get; set; }
            = null!;

        public UserRole Role { get; set; }
            = UserRole.Member;

        public string? FcmToken { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;


        public ICollection<Project> OwnedProjects
        { get; set; }
            = new List<Project>();

        public ICollection<ProjectMember> ProjectMembers
        { get; set; }
            = new List<ProjectMember>();

        public ICollection<TaskItem> CreatedTasks
        { get; set; }
            = new List<TaskItem>();

        public ICollection<TaskItem> AssignedTasks
        { get; set; }
            = new List<TaskItem>();

        public ICollection<TaskCollaborator> TaskCollaborators
        { get; set; }
            = new List<TaskCollaborator>();

        public ICollection<Comment> Comments
        { get; set; }
            = new List<Comment>();
    }
}