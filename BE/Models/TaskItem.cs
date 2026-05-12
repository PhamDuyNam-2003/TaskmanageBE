using BE.Models.Enums;

namespace BE.Models
{
    public class TaskItem
    {
        public Guid Id { get; set; }
            = Guid.NewGuid();

        public Guid? ProjectId { get; set; }

        public Project? Project { get; set; }


        public string Title { get; set; }
            = null!;

        public string? Description { get; set; }


        public Guid CreatedById { get; set; }

        public User CreatedBy { get; set; }
            = null!;


        public Guid AssignedToId { get; set; }

        public User AssignedTo { get; set; }
            = null!;


        public WorkStatus Status { get; set; }
            = WorkStatus.Todo;

        public TaskPriority Priority { get; set; }
            = TaskPriority.Medium;

        public int Progress { get; set; }
            = 0;


        public DateTime? DueDate { get; set; }


        public bool IsReminderEnabled { get; set; }
            = false;

        public DateTime? ReminderTime { get; set; }


        public bool IsDeleted { get; set; }
            = false;

        public DateTime? DeletedAt { get; set; }


        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }
            = DateTime.UtcNow;


        public byte[]? RowVersion { get; set; }


        public ICollection<SubTask> SubTasks
        { get; set; }
            = new List<SubTask>();

        public ICollection<Comment> Comments
        { get; set; }
            = new List<Comment>();

        public ICollection<TaskCollaborator> TaskCollaborators
        { get; set; } = new List<TaskCollaborator>();
    }
}