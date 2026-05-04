namespace BE.DTOs
{
   
        public record TaskDto(string Id, string Title, string Description, bool IsCompleted , int Priority );
        public class CreateTaskDto
        {
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public int Priority { get; set; } = 1;
        }
}
