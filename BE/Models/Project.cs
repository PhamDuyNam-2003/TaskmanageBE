namespace BE.Models
{
    public class Project
    {
        public Guid Id { get; set; }
            = Guid.NewGuid();

        public string Name { get; set; }
            = string.Empty;

        public string? Description { get; set; }


        public Guid OwnerId { get; set; }

        public User Owner { get; set; }
            = null!;


        public ICollection<ProjectMember> Members
        { get; set; }
            = new List<ProjectMember>();


        public ICollection<TaskItem> Tasks
        { get; set; }
            = new List<TaskItem>();


        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }
            = DateTime.UtcNow;
    }
}