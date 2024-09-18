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
        //public int Level { get; set; }
        public int ClassroomId { get; set; }
        public Classroom Classroom { get; set; }
        public Teacher? Teacher { get; set; }// Navigation property
        public Subject Subject { get; set; }  // Navigation property
        public Schedule Schedule { get; set; }  // Navigation property
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        public ICollection<Lecture> Lectures { get; set; }= new List<Lecture>();
        public ICollection<Payment> Payments { get; set; }= new List<Payment>();
        public ICollection<Certificate> Certificates { get; set; }= new List<Certificate>();
        public ICollection<CourseAnnouncement> Announcements { get; set; } = new List<CourseAnnouncement>();
    }

}
