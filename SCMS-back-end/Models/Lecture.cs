using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models
{
    public class Lecture
    {
        [Key]
        public int LectureId { get; set; }

        [Required]
        public int CourseId { get; set; }

        public DateTime LectureDate { get; set; }

        [MaxLength(255)]
        public string Topic { get; set; } = string.Empty;

        public Course Course { get; set; } = new Course(); // Navigation property
        public ICollection<LectureAttendance> LectureAttendances { get; set; } = new List<LectureAttendance>();
    }

}
