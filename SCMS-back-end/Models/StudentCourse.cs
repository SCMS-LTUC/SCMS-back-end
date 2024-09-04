using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models
{
    public class StudentCourse
    {
        [Key]
        public int StudentCourseId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int AverageGrades {  get; set; }
      //  public ICollection<Student> Student { get; set; } = new List<Student>();

       public Student Student { get; set; }  // Navigation property
        public Course Course { get; set; } // Navigation property
    }

}
