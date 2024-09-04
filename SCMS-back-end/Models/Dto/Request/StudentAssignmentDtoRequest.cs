namespace SCMS_back_end.Models.Dto.Request
{
    public class StudentAssignmentDtoRequest
    {
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public DateTime? SubmissionDate { get; set; } // Optional for teachers
        public string Submission { get; set; } = string.Empty; // Optional for teachers
        public int? Grade { get; set; } // Optional for students
        public string Feedback { get; set; } = string.Empty; // Optional for students
    }
}
