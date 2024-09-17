using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Text { get; set; } = string.Empty;

        [Required]
        public int QuizId { get; set; }

        // Navigation properties
        public Quiz Quiz { get; set; }
        public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
    }
}
