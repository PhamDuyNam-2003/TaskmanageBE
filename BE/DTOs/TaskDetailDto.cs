using BE.Models;

namespace BE.DTOs
{
    public record TaskDetailDto(
        string Id,
        string Title,
        string Description,
        string AssignedTo,
        string CreatedBy,
        int Progress,
        WorkStatus Status,
        List<Comment> Comments,
        List<SubTask> SubTasks
    );
    public record CommentDto(string UserId, string UserName, string Content, DateTime CreatedAt);
}