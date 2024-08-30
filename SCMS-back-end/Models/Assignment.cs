using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models
{
    public class Assignment
    {
        [Key]
        public int AssignmentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(255)]
        public string AssignmentName { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        public string Description { get; set; }= string.Empty;
        public bool Visible { get; set; }

        // Navigation property
        public Course Course { get; set; } = new Course(); 
        public ICollection<StudentAssignment> StudentAssignments { get; set; }= new List<StudentAssignment>();
    }

}
