﻿using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models
{
    public class StudentQuiz
    {
        [Key]
        public int StudentQuizId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int QuizId { get; set; }

        public int? Score { get; set; } // Nullable, until the quiz is graded

        public DateTime? SubmittedAt { get; set; }

        // Navigation properties
        public Student Student { get; set; }
        public Quiz Quiz { get; set; }
    }
}
