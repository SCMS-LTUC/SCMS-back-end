﻿using System.ComponentModel.DataAnnotations;

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

        public Student Student { get; set; } = new Student(); // Navigation property
        public Course Course { get; set; } = new Course(); // Navigation property
    }

}
