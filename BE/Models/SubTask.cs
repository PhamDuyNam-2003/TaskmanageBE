namespace BE.Models
{
    public class SubTask
    {
        public Guid Id { get; set; } = Guid.NewGuid();


        public Guid TaskItemId { get; set; }

        public TaskItem TaskItem { get; set; } = null!;


        public string Title { get; set; } = null!;

        public bool IsDone { get; set; } = false;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}