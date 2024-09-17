using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models
{
    public class Quiz
    {
        [Key]
        public int QuizId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        public int Duration { get; set; } // Duration in minutes

        public bool IsVisible { get; set; } // Indicates if quiz is visible to students

        // Navigation properties
        public ICollection<CourseQuiz> CourseQuizzes { get; set; } = new List<CourseQuiz>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<StudentQuiz> StudentQuizzes { get; set; } = new List<StudentQuiz>();
    }
}
