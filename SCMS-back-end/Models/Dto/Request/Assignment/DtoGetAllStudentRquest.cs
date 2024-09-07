using SCMS_back_end.Models.Dto.Response.Assignment;

namespace SCMS_back_end.Models.Dto.Request.Assignment
{
    public class DtoGetAllStudentRquest
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DtoStudentAssignmentResponse StudentAssignments { get; set; }
    }
}
