using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models
{
    public class LectureAttendance
    {
        [Key]
        public int LectureAttendanceId { get; set; }

        [Required]
        public int LectureId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [MaxLength(255)]
        public string Status { get; set; } = string.Empty;

        public Lecture Lecture { get; set; } // Navigation property
        public Student Student { get; set; }  // Navigation property
    }

}
