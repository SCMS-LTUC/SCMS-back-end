using SCMS_back_end.Models.Dto.Response.Assignment;
using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models.Dto.Request.Assignment
{
    public class DtoGetAllStudentAssignmentsRequest
    {
        [Key]
        public int AssignmentId { get; set; }

        //[Required]
        //public int CourseId { get; set; }

        [Required]
        [MaxLength(255)]
        public string AssignmentName { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

       public ICollection<DtoStudentAssignmentResponse> StudentAssignments { get; set; } = new List<DtoStudentAssignmentResponse>();
    }
}
