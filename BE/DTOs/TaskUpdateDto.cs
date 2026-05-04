using BE.Models;

namespace BE.DTOs
{
    public class TaskUpdateProgressDto
    {
        public int Progress { get; set; }
        public WorkStatus Status { get; set; }
    }

    public class AddCommentDto
    {
        public string Content { get; set; } = null!;
    }
}
