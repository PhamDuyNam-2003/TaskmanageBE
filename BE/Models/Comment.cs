namespace BE.Models
{
    public class Comment
    {
        public Guid Id { get; set; } = Guid.NewGuid();


        public Guid TaskItemId { get; set; }

        public TaskItem TaskItem { get; set; } = null!;


        public Guid UserId { get; set; }

        public User User { get; set; } = null!;


        public string Content { get; set; } = null!;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}