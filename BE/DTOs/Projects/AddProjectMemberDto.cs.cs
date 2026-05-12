using System;
using System.Collections.Generic;
using System.Text;
using BE.Models.Enums;

    namespace BE.DTOs.Projects
    {
        public class AddProjectMemberDto
        {
            public Guid UserId { get; set; }

            public ProjectRole Role { get; set; }
        }
    }

