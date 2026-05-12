using BE.Models.Enums;

namespace BE.Models
{
    public class ProjectMember
    {
        public Guid ProjectId { get; set; }

        public Project Project { get; set; }
            = null!;

        public Guid UserId { get; set; }

        public User User { get; set; }
            = null!;

        public ProjectRole Role { get; set; }
            = ProjectRole.Member;

        public DateTime JoinedAt { get; set; }
            = DateTime.UtcNow;
    }
}