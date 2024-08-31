﻿using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }
        public string UserId { get; set; } = string.Empty;

        public int Level { get; set; }

        [Required]
        [MaxLength(255)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string PhoneNumber { get; set; } = string.Empty;

        public User User { get; set; } = new User(); // Navigation property
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
        public ICollection<StudentAssignment> StudentAssignments { get; set; } = new List<StudentAssignment>();
        public ICollection<LectureAttendance> LectureAttendances { get; set; } = new List<LectureAttendance>();
    }

}
