using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models
{
    public class AnswerOption
    {
        [Key]
        public int AnswerOptionId { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        public bool IsCorrect { get; set; } // Indicates if the answer is correct

        [Required]
        public int QuestionId { get; set; }

        // Navigation property
        public Question Question { get; set; }
    }
}
