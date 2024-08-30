using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models
{
    public class StudentAssignment
    {
        [Key]
        public int StudentAssignmentId { get; set; }

        [Required]
        public int AssignmentId { get; set; }

        [Required]
        public int StudentId { get; set; }

        public DateTime SubmissionDate { get; set; }

        public int Grade { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public string Submission { get; set; }= string.Empty;

        public Assignment Assignment { get; set; } = new Assignment(); // Navigation property
        public Student Student { get; set; } = new Student(); // Navigation property
    }

}
