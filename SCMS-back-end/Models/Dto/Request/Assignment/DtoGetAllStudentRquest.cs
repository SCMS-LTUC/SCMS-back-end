using SCMS_back_end.Models.Dto.Response.Assignment;

namespace SCMS_back_end.Models.Dto.Request.Assignment
{
    public class DtoGetAllStudentRquest
    {


        public int StudentId { get; set; }

        public DateTime SubmissionDate { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int Grade { get; set; }
        public string Feedback { get; set; } = string.Empty;

        public ICollection<DtoStudentAssignmentResponse> StudentAssignments { get; set; } = new List<DtoStudentAssignmentResponse>();

        public Student Student { get; set;}
    }
}
