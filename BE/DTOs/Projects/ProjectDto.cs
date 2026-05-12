using System;
using System.Collections.Generic;

namespace BE.DTOs.Projects
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<ProjectMemberDto> Members { get; set; } = new();
    }
}
