namespace BE.DTOs.Projects
{
    public class ProjectMemberDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int Role { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
