
using BE.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class SubTask
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public bool IsDone { get; set; } = false;
}
public class TaskItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? ProjectId { get; set; } // Thuộc dự án nào?

    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    // PHÂN VIỆC CỤ THỂ
    [BsonRepresentation(BsonType.ObjectId)]
    public string CreatedBy { get; set; } = null!; // Người giao

    [BsonRepresentation(BsonType.ObjectId)]
    public string AssignedTo { get; set; } = null!; // Người thực hiện chính

    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> CollaboratorIds { get; set; } = new(); // Người cùng làm

    public WorkStatus Status { get; set; } = WorkStatus.Todo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public int Progress { get; set; } = 0;

    public List<SubTask> SubTasks { get; set; } = new();
    public List<Comment> Comments { get; set; } = new(); // Trao đổi trong Task

    public DateTime? DueDate { get; set; }
    public bool IsReminderEnabled { get; set; } = false;
    public DateTime? ReminderTime { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class Comment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = null!; // Ai comment
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}