namespace SCMS_back_end.Models.Dto.Request
{
    // DTO for Teacher Assignment Feedback
    public class TeacherAssignmentFeedbackDtoRequest
    {
        public int StudentAssignmentId { get; set; }
        public int? Grade { get; set; }
        public string Feedback { get; set; } = string.Empty;
    }
}
