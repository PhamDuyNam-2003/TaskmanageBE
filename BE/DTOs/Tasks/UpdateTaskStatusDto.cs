using System;
using System.Collections.Generic;
using System.Text;

using BE.Models.Enums;

namespace BE.DTOs.Tasks
{
    public class UpdateTaskStatusDto
    {
        public WorkStatus Status { get; set; }
    }
}