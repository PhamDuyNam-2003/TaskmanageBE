using BE.Models.Enums;

namespace BE.DTOs.Tasks
{
    public class UpdateTaskDto
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public WorkStatus Status { get; set; }

        public TaskPriority Priority { get; set; }

        public int Progress { get; set; }

        public DateTime? DueDate { get; set; }
    }
}