using BE.Models.Enums;

namespace BE.DTOs.Tasks
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public Guid? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        public Guid AssignedToId { get; set; }
        public string? AssignedToName { get; set; }
        public WorkStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public int Progress { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<SubTaskDto> SubTasks { get; set; } = new();
        public List<CommentDto> Comments { get; set; } = new();
    }
}
