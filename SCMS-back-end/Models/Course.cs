using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        public int? TeacherId { get; set; }

        [Required]
        public int SubjectId { get; set; } 

        [Required]
        public int ScheduleId { get; set; }

        [MaxLength(255)]
        public string ClassName { get; set; } = string.Empty;

        public int Capacity { get; set; }
        public int Level { get; set; }

        public Teacher? Teacher { get; set; }// Navigation property
        public Subject Subject { get; set; }  // Navigation property
        public Schedule Schedule { get; set; }  // Navigation property
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        public ICollection<Lecture> Lectures { get; set; }= new List<Lecture>();
    }

}
