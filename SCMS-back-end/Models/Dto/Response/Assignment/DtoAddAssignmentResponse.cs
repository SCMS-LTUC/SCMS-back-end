﻿namespace SCMS_back_end.Models.Dto.Response.Assignment
{
    public class DtoAddAssignmentResponse
    {
        public string AssignmentName { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        public string Description { get; set; } = string.Empty;

        public bool Visible { get; set; }
    }
}
