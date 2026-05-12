namespace BE.Models
{
    public class TaskCollaborator
    {
        public Guid TaskItemId { get; set; }

        public TaskItem TaskItem { get; set; }
            = null!;


        public Guid UserId { get; set; }

        public User User { get; set; }
            = null!;


        public DateTime JoinedAt { get; set; }
            = DateTime.UtcNow;

    }
}