using BE.Models.Enums;

namespace BE.DTOs.Tasks
{
    public class CreateTaskDto
    {
        public Guid? ProjectId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public Guid AssignedToId { get; set; }

        public TaskPriority Priority { get; set; }

        public int Progress { get; set; }
        public DateTime? DueDate { get; set; }
    }
}