using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models.Dto.Response.Assignment
{
    public class DtoStudentAssignmentResponse
    {
        [Key]
        public int StudentAssignmentId { get; set; }
        public string AssignmentName { get; set; } = string.Empty;
        public int Grade { get; set; }
        public string Feedback { get; set; } = string.Empty;
      
    }
}
